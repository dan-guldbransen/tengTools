using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Litium.Blobs;
using Litium.Customers;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Media;
using Litium.Media.Queryable;
using Litium.Security;
using File = Litium.Media.File;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Media archive structure package
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public class MediaArchiveStructurePackage : IStructurePackage<StructureInfo.MediaArchiveStructure>
    {
        public const string ModuleName = "media";
        private readonly FileService _fileService;
        private readonly FolderService _folderService;
        private readonly DataService _dataService;
        private readonly FileMetadataExtractorService _fileMetadataExtractorService;
        private readonly BlobService _blobService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly GroupService _groupService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public MediaArchiveStructurePackage(FileService fileService, FolderService folderService, DataService dataService, FileMetadataExtractorService fileMetadataExtractorService, BlobService blobService, FieldTemplateService fieldTemplateService, GroupService groupService)
        {
            _folderService = folderService;
            _fileService = fileService;
            _dataService = dataService;
            _fileMetadataExtractorService = fileMetadataExtractorService;
            _blobService = blobService;
            _fieldTemplateService = fieldTemplateService;
            _groupService = groupService;
        }

        /// <summary>
        ///     Exports the specified package info.
        /// </summary>
        /// <param name="packageInfo"> The package info. </param>
        /// <returns> </returns>
        public virtual StructureInfo.MediaArchiveStructure Export(PackageInfo packageInfo)
        {
            var structure = new StructureInfo.MediaArchiveStructure
            {
                FileData = new Dictionary<Guid, byte[]>(),
                Files = new List<Media.File>(),
                Folders = new List<Folder>(),
            };

            var visitorGroupId = (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare")).SystemId;
            ExportFolders(structure, packageInfo.Folder, visitorGroupId);

            var folderId = packageInfo.Folder.SystemId;
            structure.Folders
                     .FindAll(x => x.ParentFolderSystemId == folderId)
                     .ForEach(x => x.ParentFolderSystemId = Guid.Empty);

            structure.Files
                     .FindAll(x => x.FolderSystemId == folderId)
                     .ForEach(x => x.FolderSystemId = Guid.Empty);

            return structure;
        }

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        public virtual void Import(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            Import(structureInfo, packageInfo.Folder, Guid.Empty);
        }

        /// <summary>
        ///     Prepares the import.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        public virtual void PrepareImport(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            structureInfo.Mappings.Add(packageInfo.Folder.SystemId, Guid.Empty);
            foreach (var item in structureInfo.MediaArchive.Folders)
            {
                structureInfo.Mappings.Add(item.SystemId, Guid.NewGuid());
            }
            foreach (var item in structureInfo.MediaArchive.Files)
            {
                structureInfo.Mappings.Add(item.SystemId, Guid.NewGuid());
            }
            structureInfo.MediaArchive.FolderTemplateId = _fieldTemplateService.GetAll().First(c => (c is FolderFieldTemplate)).SystemId;
        }

        /// <summary>
        ///     Exports the files.
        /// </summary>
        /// <param name="structure"> The structure. </param>
        /// <param name="folder"> The folder. </param>
        protected virtual void ExportFiles(StructureInfo.MediaArchiveStructure structure, Folder folder)
        {
            List<File> files;
            using (var query = _dataService.CreateQuery<File>())
            {
                files = query.Filter(f => f.FolderSystemId(folder.SystemId)).ToList();
            }

            foreach (var item in files)
            {
                structure.Files.Add(item.MakeWritableClone());
                structure.FileData[item.SystemId] = item.GetFileContent();
            }
        }

        /// <summary>
        ///     Exports the folders.
        /// </summary>
        /// <param name="structure"> The structure. </param>
        /// <param name="folder"> The folder. </param>
        protected virtual void ExportFolders(StructureInfo.MediaArchiveStructure structure, Folder folder, Guid visitorGroupId)
        {
            foreach (var item in _folderService.GetChildFolders(folder.SystemId))
            {
                var clone = item.MakeWritableClone();
                var visitorGroupPermissions = item.AccessControlList.Where(x => x.GroupSystemId == visitorGroupId);
                clone.AccessControlList = new HashSet<AccessControlEntry>(visitorGroupPermissions);
                structure.Folders.Add(clone);
                ExportFolders(structure, item, visitorGroupId);
            }

            ExportFiles(structure, folder);
        }

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="folder"> The folder. </param>
        /// <param name="parentId"> The parent id. </param>
        protected virtual void Import(StructureInfo structureInfo, Folder folder, Guid parentId)
        {
            foreach (var folderCarrier in structureInfo.MediaArchive.Folders.Where(x => x.ParentFolderSystemId == parentId))
            {
                var newFolder = new Folder(structureInfo.MediaArchive.FolderTemplateId, folderCarrier.Name)
                {
                    SystemId = structureInfo.Id(folderCarrier.SystemId),
                    ParentFolderSystemId = folder.SystemId,
                };

                foreach (var item in folderCarrier.AccessControlList)
                {
                    newFolder.AccessControlList.Add(new AccessControlEntry(item.Operation, structureInfo.Id(structureInfo.Foundation.VisitorGroupId)));
                }
                _folderService.Create(newFolder);
                if (structureInfo.CreateExampleProducts)
                {
                    Import(structureInfo, newFolder, folderCarrier.SystemId);
                }
                else
                {
                    ImportFiles(structureInfo, newFolder, folderCarrier.SystemId);
                }
            }

            ImportFiles(structureInfo, folder, parentId);
        }

        protected virtual void ImportFiles(StructureInfo structureInfo, Folder folder, Guid parentId)
        {
            foreach (var fileCarrier in structureInfo.MediaArchive.Files.Where(x => x.FolderSystemId == parentId).OrderBy(x => x.Name))
            {
                using (var stream = new MemoryStream(structureInfo.MediaArchive.FileData[fileCarrier.SystemId]))
                {
                    var blobContainer = _blobService.Create(ModuleName);
                    using (var blobStream = blobContainer.GetDefault().OpenWrite())
                    {
                        stream.CopyTo(blobStream);
                    }
                    var template = _fieldTemplateService.FindFileTemplate(fileCarrier.GetFileExtension());
                    var fileObject = new File(template.SystemId, folder.SystemId, blobContainer.Uri, fileCarrier.Name)
                    {
                        SystemId = structureInfo.Id(fileCarrier.SystemId),
                        LastWriteTimeUtc = DateTimeOffset.UtcNow
                    };
                    _fileMetadataExtractorService.UpdateMetadata(template, fileObject, null, blobContainer.Uri);
                    fileObject.AccessControlList = folder.AccessControlList;
                    _fileService.Create(fileObject);
                }
            }
        }
    }
}

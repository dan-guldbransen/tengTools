using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Litium.Accelerator.Definitions;
using Litium.Accelerator.Deployments;
using Litium.Accelerator.ViewModels.Deployment;
using Litium.Globalization;
using Litium.Media;
using Litium.Security;
using Litium.Studio.Extenssions;

namespace Litium.Accelerator.Builders.Deployment
{
    public class DeploymentViewModelBuilder : IViewModelBuilder<DeploymentViewModel>
    {
        private readonly ChannelService _channelService;
        private readonly FolderService _folderService;
        private readonly IPackage _package;
        private readonly AuthorizationService _authorizationService;

        public DeploymentViewModelBuilder(ChannelService channelService, FolderService folderService, IPackage package, AuthorizationService authorizationService)
        {
            _channelService = channelService;
            _folderService = folderService;
            _package = package;
            _authorizationService = authorizationService;
        }

        public virtual DeploymentViewModel Build(bool showExport = false)
        {
            var model = new DeploymentViewModel
            {
                ShowImport = true,
                ShowExport = showExport,
            };

            if (model.ShowImport)
            {
                model.CanImport = 
                    _authorizationService.HasOperation(Operations.Function.Media.Content)
                    && _authorizationService.HasOperation(Operations.Function.Media.Settings)
                    && _authorizationService.HasOperation(Operations.Function.Products.Content)
                    && _authorizationService.HasOperation(Operations.Function.Products.Settings)
                    && _authorizationService.HasOperation(Operations.Function.Websites.Content)
                    && _authorizationService.HasOperation(Operations.Function.Websites.Settings);
                model.Packages = GetPackages();

                if (model.Packages.Count == 0)
                {
                    model.CanImport = false;
                }

                if (model.CanImport && DefinitionSetup.DefinitionsError)
                {
                    model.CanImport = false;
                    model.ImportMessage = "accelerator.deployment.field.definitions.notmatching".AsAngularResourceString();
                }
            }

            if (model.ShowExport)
            {
                model.CanExport = true;
                model.Channels = GetChannels();
                model.Folders = GetFolders();

                if (!model.Channels.Any() || !model.Folders.Any())
                {
                    model.CanExport = false;
                }
            }

            return model;
        }

        private IEnumerable<SelectListItem> GetChannels()
        {
            var channels = _channelService.GetAll().Where(c => c.WebsiteSystemId != null && c.WebsiteSystemId != Guid.Empty);

            return channels.Select(channel => new SelectListItem
            {
                Text = channel.Localizations.CurrentCulture.Name,
                Value = channel.SystemId.ToString()
            });
        }

        private IEnumerable<SelectListItem> GetFolders()
        {
            var folders = _folderService.GetChildFolders(Guid.Empty);

            return folders.Select(folder => new SelectListItem
            {
                Text = folder.Name,
                Value = folder.SystemId.ToString()
            });
        }

        private IList<SelectListItem> GetPackages()
        {
            var folder = new DirectoryInfo(_package.GetPackageStoragedPath());
            if (folder.Exists)
            {
                var files = folder.GetFiles("*.bin").Select(x => x.Name.Substring(0, x.Name.IndexOf('.'))).Distinct();

                return files.Select(file => new SelectListItem
                {
                    Text = file,
                    Value = file.Replace("Accelerator_","")
                }).ToList();
            }

            return new List<SelectListItem>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Foundation.Modules.ProductCatalog;
using Litium.Globalization;

namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Package service
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public class PackageService : IPackageService
    {
        private readonly IStructurePackage<StructureInfo.WebsiteStructure> _websiteStructurePackage;
        private readonly IStructurePackage<StructureInfo.ECommerceStructure> _ecommerceStructurePackage;
        private readonly IStructurePackage<StructureInfo.MediaArchiveStructure> _mediaStructurePackage;
        private readonly IStructurePackage<StructureInfo.ProductCatalogStructure> _productStructurePackage;
        private readonly GroupService _groupService;
        private readonly LanguageService _languageService;
        private readonly CurrencyService _currencyService;
        private readonly FieldTemplateService _fieldTemplateService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public PackageService(IStructurePackage<StructureInfo.WebsiteStructure> websiteStructurePackage,
            IStructurePackage<StructureInfo.MediaArchiveStructure> mediaStructurePackage,
            IStructurePackage<StructureInfo.ProductCatalogStructure> productStructurePackage,
            IStructurePackage<StructureInfo.ECommerceStructure> ecommerceStructurePackage,
            GroupService groupService, LanguageService languageService, CurrencyService currencyService, FieldTemplateService fieldTemplateService)
        {
            _websiteStructurePackage = websiteStructurePackage;
            _mediaStructurePackage = mediaStructurePackage;
            _productStructurePackage = productStructurePackage;
            _ecommerceStructurePackage = ecommerceStructurePackage;
            _groupService = groupService;
            _languageService = languageService;
            _currencyService = currencyService;
            _fieldTemplateService = fieldTemplateService;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Exports to structure info.
        /// </summary>
        /// <param name="packageInfo"> The package info. </param>
        /// <returns> </returns>
        public StructureInfo Export(PackageInfo packageInfo)
        {
            var languagedMap = _languageService.GetAll().ToDictionary(x => x.SystemId, x => x.CultureInfo.Name);
            var visitorGroupId = (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare")).SystemId;
            var currencies = _currencyService.GetAll().ToList();
            var relationTemplates = _fieldTemplateService.GetAll().OfType<Customers.FieldTemplateBase>().ToDictionary(x => x.SystemId, x => x.Id);
            var website = _websiteStructurePackage.Export(packageInfo);
            var mediaArchive = _mediaStructurePackage.Export(packageInfo);
            var productCatalog = _productStructurePackage.Export(packageInfo);
            var ecommerce = _ecommerceStructurePackage.Export(packageInfo);
            var structureInfo = new StructureInfo
            {
                Foundation = new StructureInfo.FoundationStructure
                {
                    LanguageMap = languagedMap,
                    VisitorGroupId = visitorGroupId,
                    Currencies = currencies,
                    RelationTemplates = relationTemplates
                },
                Website = website,
                MediaArchive = mediaArchive,
                ProductCatalog = productCatalog,
                ECommerce = ecommerce
            };
            return structureInfo;
        }

        /// <summary>
        ///     Imports the specified structure info.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        public void Import(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            Prepare(structureInfo, packageInfo);
            _mediaStructurePackage.Import(structureInfo, packageInfo);
            _productStructurePackage.Import(structureInfo, packageInfo);
            _ecommerceStructurePackage.Import(structureInfo, packageInfo);
            _websiteStructurePackage.Import(structureInfo, packageInfo);
        }

        /// <summary>
        ///     Prepares the specified structure info.
        /// </summary>
        /// <param name="structureInfo"> The structure info. </param>
        /// <param name="packageInfo"> The package info. </param>
        protected void Prepare(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            _mediaStructurePackage.PrepareImport(structureInfo, packageInfo);
            _productStructurePackage.PrepareImport(structureInfo, packageInfo);
            _ecommerceStructurePackage.PrepareImport(structureInfo, packageInfo);
            _websiteStructurePackage.PrepareImport(structureInfo, packageInfo);
        }
    }
}


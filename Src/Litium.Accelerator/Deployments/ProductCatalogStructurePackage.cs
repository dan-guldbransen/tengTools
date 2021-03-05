using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Search.Filtering;
using Litium.ComponentModel;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ExtensionMethods;
using Litium.Globalization;
using Litium.Products;


namespace Litium.Accelerator.Deployments
{
    /// <summary>
    ///     Product catalog structure package.
    /// </summary>
    /// <remarks>
    ///     Disclaimer: Class is still under development and can be changed without notification and with breaking changes.
    /// </remarks>
    public class ProductCatalogStructurePackage : IStructurePackage<StructureInfo.ProductCatalogStructure>
    {
        private readonly BaseProductService _baseProductService;
        private readonly List<ImportBidirectionalRelation> _bidirectionalRelationList;
        private readonly CategoryService _categoryService;
        private readonly DataService _dataService;
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly InventoryService _inventoryService;
        private readonly LanguageService _languageService;
        private readonly PriceListService _priceListService;
        private readonly ProductListService _productListService;
        private readonly RelationshipTypeService _relationshipTypeService;
        private readonly UnitOfMeasurementService _unitOfMeasurementService;
        private readonly VariantService _variantService;
        private readonly StructureInfoService _structureInfoService;
        private readonly CurrencyService _currencyService;
        private readonly FilterService _filterService;
        private readonly InventoryItemService _inventoryItemService;
        private readonly PriceListItemService _priceListItemService;
        private readonly ProductListItemService _productListItemService;

        public ProductCatalogStructurePackage(BaseProductService baseProductService, ProductListService productListService,
            RelationshipTypeService relationshipTypeService, CategoryService categoryService,
            UnitOfMeasurementService unitOfMeasurementService, DataService dataService,
            FieldTemplateService fieldTemplateService,
            LanguageService languageService, VariantService variantService,
            InventoryService inventoryService,
            PriceListService priceListService,
            StructureInfoService structureInfoService,
            CurrencyService currencyService,
            FilterService filterService,
            InventoryItemService inventoryItemService,
            PriceListItemService priceListItemService,
            ProductListItemService productListItemService)
        {
            _baseProductService = baseProductService;
            _categoryService = categoryService;
            _dataService = dataService;
            _fieldTemplateService = fieldTemplateService;
            _languageService = languageService;
            _variantService = variantService;
            _inventoryService = inventoryService;
            _priceListService = priceListService;
            _structureInfoService = structureInfoService;
            _currencyService = currencyService;
            _filterService = filterService;
            _productListService = productListService;
            _relationshipTypeService = relationshipTypeService;
            _unitOfMeasurementService = unitOfMeasurementService;
            _bidirectionalRelationList = new List<ImportBidirectionalRelation>();
            _inventoryItemService = inventoryItemService;
            _priceListItemService = priceListItemService;
            _productListItemService = productListItemService;
        }

        public StructureInfo.ProductCatalogStructure Export(PackageInfo packageInfo)
        {
            var structure = new StructureInfo.ProductCatalogStructure
            {
                Assortment = packageInfo.Assortment,
                Variants = new List<Variant>(),
                Categories = new List<Category>(),
                BaseProducts = new List<BaseProduct>(),
                ProductLists = _productListService.GetAll().ToList(),
                TemplateMap = _fieldTemplateService.GetAll().ToList().ToDictionary(x => x.SystemId, x => x.Id),
                RelationTypeMap = _relationshipTypeService.GetAll().ToList()
                                                          .ToDictionary(x => x.SystemId, x => x.Id),
                ProductFilters = _filterService.GetProductFilteringFields().ToList()

            };
            using (var query = _dataService.CreateQuery<BaseProduct>())
            {
                var products = query.ToList();
                foreach (var product in products)
                {
                    var linkedCategory = _categoryService.GetByBaseProduct(product.SystemId);
                    if (linkedCategory.All(x => x.AssortmentSystemId != packageInfo.Assortment?.SystemId))
                    {
                        continue;
                    }
                    var variants = _variantService.GetByBaseProduct(product.SystemId).ToList();
                    if (!variants.Any(x => x.ChannelLinks.Any(w => w.ChannelSystemId == packageInfo.Channel.SystemId)))
                    {
                        continue;
                    }
                    structure.Variants.AddRange(variants);
                    structure.BaseProducts.Add(product);
                }
            }

            var rootCategories = _categoryService.GetChildCategories(Guid.Empty, packageInfo.Assortment?.SystemId);
            foreach (var rootCategory in rootCategories)
            {
                structure.Categories.AddRange(rootCategory.GetChildren(true));
            }

            structure.PriceListItems = structure.Variants.SelectMany(x => _priceListItemService.GetByVariant(x.SystemId)).ToList();
            structure.ProductListItems = structure.BaseProducts.SelectMany(x => _productListItemService.GetByBaseProduct(x.SystemId)).ToList();

            var priceList = _priceListService.GetAll().FirstOrDefault(x => x.WebSiteLinks.Any(l => l.WebSiteSystemId == packageInfo.Website.SystemId));
            if (priceList != null)
            {
                structure.PriceList = priceList;
                structure.CurrencyId = _currencyService.Get(priceList.CurrencySystemId)?.Id;
            }

            structure.InventoryItems = structure.Variants.SelectMany(x => _inventoryItemService.GetByVariant(x.SystemId))
                .GroupBy(x => x.InventorySystemId)
                .OrderByDescending(x => x.Count())
                .Take(1)
                .SelectMany(x => x)
                .ToList();

            var inventory = _inventoryService.Get(structure.InventoryItems.Count == 0 ? Guid.Empty : structure.InventoryItems[0].InventorySystemId);
            if (inventory != null)
            {
                structure.Inventory = inventory;
            }
            structure.UnitOfMeasurements = _unitOfMeasurementService.GetAll().ToList();

            return structure;
        }

        public void Import(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            structureInfo.Prefix = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

            ImportProducts(structureInfo);
            ImportCategories(structureInfo);
        }

        public void PrepareImport(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            structureInfo.Mappings.Add(packageInfo.Assortment.SystemId, Guid.Empty);
            if (structureInfo.ProductCatalog.Assortment != null)
            {
                structureInfo.Mappings.Add(structureInfo.ProductCatalog.Assortment.SystemId, packageInfo.Assortment.SystemId);
            }

            if (!structureInfo.CreateExampleProducts)
            {
                structureInfo.ProductCatalog.Categories.RemoveRange(0, structureInfo.ProductCatalog.Categories.Count);
                structureInfo.ProductCatalog.PriceListItems.Clear();
                structureInfo.ProductCatalog.InventoryItems.Clear();
                structureInfo.ProductCatalog.Variants.Clear();
                structureInfo.ProductCatalog.ProductLists.Clear();
                structureInfo.ProductCatalog.UnitOfMeasurements.Clear();
                structureInfo.ProductCatalog.BaseProducts.Clear();
            }

            foreach (var item in structureInfo.ProductCatalog.Variants)
            {
                structureInfo.Mappings.Add(item.SystemId, Guid.NewGuid());
            }
            foreach (var item in structureInfo.ProductCatalog.Categories)
            {
                structureInfo.Mappings.Add(item.SystemId, Guid.NewGuid());
            }
            foreach (var item in structureInfo.ProductCatalog.BaseProducts)
            {
                structureInfo.Mappings.Add(item.SystemId, Guid.NewGuid());
            }
            foreach (var item in structureInfo.ProductCatalog.ProductLists)
            {
                var existProductList = _productListService.Get<ProductList>(item.Id);
                if (existProductList == null)
                {
                    var newId = Guid.NewGuid();
                    structureInfo.Mappings.Add(item.SystemId, newId);
                    var productList = new StaticProductList();
                    productList.SystemId = newId;
                    productList.Id = item.Id?.Replace(" ", "");
                    productList.AccessControlList = item.AccessControlList.MakeWritable();
                    foreach (var localizationItem in item.Localizations)
                    {
                        productList.Localizations[localizationItem.Key].Name = localizationItem.Value?.Name;
                    }

                    _productListService.Create(productList);
                }
                else
                {
                    structureInfo.Mappings.Add(item.SystemId, existProductList.SystemId);
                }
            }
            foreach (var item in structureInfo.ProductCatalog.TemplateMap)
            {
                var templateSytemId = _fieldTemplateService.Get<ProductFieldTemplate>(item.Value)?.SystemId ?? _fieldTemplateService.Get<CategoryFieldTemplate>(item.Value)?.SystemId ?? Guid.Empty;

                if (templateSytemId != Guid.Empty)
                {
                    structureInfo.Mappings.Add(item.Key, templateSytemId);
                }
            }

            foreach (var item in structureInfo.ProductCatalog.UnitOfMeasurements)
            {
                var existUnitOfMeasurement = _unitOfMeasurementService.Get(item.Id);
                if (existUnitOfMeasurement == null)
                {
                    var newId = Guid.NewGuid();
                    structureInfo.Mappings.Add(item.SystemId, newId);
                    var unitOfMeasurement = new UnitOfMeasurement(item.Id);
                    unitOfMeasurement.SystemId = newId;
                    unitOfMeasurement.DecimalDigits = item.DecimalDigits;
                    foreach (var localizationItem in item.Localizations)
                    {
                        unitOfMeasurement.Localizations[localizationItem.Key].Name = localizationItem.Value?.Name;
                    }

                    _unitOfMeasurementService.Create(unitOfMeasurement);
                }
                else
                {
                    structureInfo.Mappings.Add(item.SystemId, existUnitOfMeasurement.SystemId);
                }
            }

            foreach (var item in structureInfo.ProductCatalog.RelationTypeMap)
            {
                var relation = _relationshipTypeService.Get(item.Value);
                if (relation != null)
                {
                    structureInfo.Mappings.Add(item.Key, relation.SystemId);
                }
            }
        }


        private void AddBundles(StructureInfo structureInfo, Variant variant, Variant newVariant)
        {
            if (variant.BundledVariants != null && variant.BundledVariants.Count > 0)
            {
                newVariant.BundledVariants = new List<VariantBundledLink>();
                foreach (var bundleItem in variant.BundledVariants)
                {
                    if (structureInfo.Id(bundleItem.BundledVariantSystemId) != bundleItem.BundledVariantSystemId)
                    {
                        newVariant.BundledVariants.Add(new VariantBundledLink(structureInfo.Id(bundleItem.BundledVariantSystemId)) { Quantity = bundleItem.Quantity });
                    }
                }
            }
        }

        private void AddInventories(StructureInfo structureInfo)
        {
            if (structureInfo.ProductCatalog.InventoryItems != null && structureInfo.ProductCatalog.InventoryItems.Count > 0)
            {
                foreach (var inventoryItem in structureInfo.ProductCatalog.InventoryItems)
                {
                    _inventoryItemService.Create(
                        new InventoryItem(
                            structureInfo.Id(inventoryItem.VariantSystemId),
                            structureInfo.Id(inventoryItem.InventorySystemId))
                        {
                            InStockQuantity = inventoryItem.InStockQuantity
                        });
                }
            }
        }

        private void AddPrices(StructureInfo structureInfo)
        {
            if (structureInfo.ProductCatalog.PriceListItems != null && structureInfo.ProductCatalog.PriceListItems.Count > 0)
            {
                foreach (var priceItem in structureInfo.ProductCatalog.PriceListItems)
                {
                    _priceListItemService.Create(new PriceListItem(structureInfo.Id(priceItem.VariantSystemId), structureInfo.Id(priceItem.PriceListSystemId))
                    {
                        MinimumQuantity = priceItem.MinimumQuantity,
                        Price = priceItem.Price,
                    });
                }
            }
        }

        private void AddProductLists(StructureInfo structureInfo)
        {
            using (var batch = _dataService.CreateBatch())
            {
                foreach (var productListLink in structureInfo.ProductCatalog.ProductListItems)
                {
                    if (_productListService.Get<ProductList>(structureInfo.Id(productListLink.ProductListSystemId)) is object)
                    {
                        var item = new ProductListItem(structureInfo.Id(productListLink.BaseProductSystemId), structureInfo.Id(productListLink.ProductListSystemId));
                        foreach (var activeVariantSystemId in productListLink.ActiveVariantSystemIds)
                        {
                            item.ActiveVariantSystemIds.Add(structureInfo.Id(activeVariantSystemId));
                        }
                        batch.Create(item);
                    }
                }
                batch.Commit();
            }
        }

        private void AddRelations(StructureInfo structureInfo, BaseProduct baseProduct, BaseProduct newBaseProduct)
        {
            if (baseProduct.RelationshipLinks != null && baseProduct.RelationshipLinks.Count > 0)
            {
                if (newBaseProduct.RelationshipLinks == null)
                {
                    newBaseProduct.RelationshipLinks = new List<BaseProductToRelationshipLinkBase>();
                }
                foreach (var relationshipLink in baseProduct.RelationshipLinks)
                {
                    var relationshipType =
                        _relationshipTypeService.Get(structureInfo.Id(relationshipLink.RelationshipTypeSystemId));
                    var relatedItemId = Guid.Empty;
                    if (relationshipLink is BaseProductToBaseProductRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(
                                ((BaseProductToBaseProductRelationshipLink)relationshipLink).BaseProductSystemId);
                        relatedItemId = newId ==
                                        ((BaseProductToBaseProductRelationshipLink)relationshipLink)
                                        .BaseProductSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is BaseProductToVariantRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((BaseProductToVariantRelationshipLink)relationshipLink).VariantSystemId);
                        relatedItemId = newId ==
                                        ((BaseProductToVariantRelationshipLink)relationshipLink).VariantSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is BaseProductToCategoryRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((BaseProductToCategoryRelationshipLink)relationshipLink).CategorySystemId);
                        relatedItemId = newId ==
                                        ((BaseProductToCategoryRelationshipLink)relationshipLink).CategorySystemId
                            ? Guid.Empty
                            : newId;
                    }
                    if (relatedItemId != Guid.Empty)
                    {
                        if (relationshipType.Bidirectional)
                        {
                            //Avoid adding duplicates
                            var relationWasAdded =
                                _bidirectionalRelationList.Any(x => x.RelationTypeId == relationshipType.SystemId &&
                                                                    x.Item1Id == relatedItemId &&
                                                                    x.Item2Id == newBaseProduct.SystemId);
                            if (relationWasAdded)
                            {
                                continue;
                            }

                            _bidirectionalRelationList.Add(new ImportBidirectionalRelation
                            {
                                RelationTypeId = relationshipType.SystemId,
                                Item1Id = newBaseProduct.SystemId,
                                Item2Id = relatedItemId
                            });
                        }

                        if (relationshipLink is BaseProductToBaseProductRelationshipLink)
                        {
                            newBaseProduct.RelationshipLinks.Add(
                                new BaseProductToBaseProductRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is BaseProductToVariantRelationshipLink)
                        {
                            newBaseProduct.RelationshipLinks.Add(
                                new BaseProductToVariantRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is BaseProductToCategoryRelationshipLink)
                        {
                            newBaseProduct.RelationshipLinks.Add(
                                new BaseProductToCategoryRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                    }
                }
            }
        }

        private void AddRelations(StructureInfo structureInfo, Category category, Category newCategory)
        {
            if (category.RelationshipLinks != null && category.RelationshipLinks.Count > 0)
            {
                if (newCategory.RelationshipLinks == null)
                {
                    newCategory.RelationshipLinks = new List<CategoryToRelationshipLinkBase>();
                }
                foreach (var relationshipLink in category.RelationshipLinks)
                {
                    var relationshipType =
                        _relationshipTypeService.Get(structureInfo.Id(relationshipLink.RelationshipTypeSystemId));
                    var relatedItemId = Guid.Empty;
                    if (relationshipLink is CategoryToBaseProductRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(
                                ((CategoryToBaseProductRelationshipLink)relationshipLink).BaseProductSystemId);
                        relatedItemId = newId ==
                                        ((CategoryToBaseProductRelationshipLink)relationshipLink).BaseProductSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is CategoryToVariantRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((CategoryToVariantRelationshipLink)relationshipLink).VariantSystemId);
                        relatedItemId = newId == ((CategoryToVariantRelationshipLink)relationshipLink).VariantSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is CategoryToCategoryRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((CategoryToCategoryRelationshipLink)relationshipLink).CategorySystemId);
                        relatedItemId = newId ==
                                        ((CategoryToCategoryRelationshipLink)relationshipLink).CategorySystemId
                            ? Guid.Empty
                            : newId;
                    }
                    if (relatedItemId != Guid.Empty)
                    {
                        if (relationshipType.Bidirectional)
                        {
                            //Avoid adding duplicates
                            var relationWasAdded =
                                _bidirectionalRelationList.Any(x => x.RelationTypeId == relationshipType.SystemId &&
                                                                    x.Item1Id == relatedItemId &&
                                                                    x.Item2Id == newCategory.SystemId);
                            if (relationWasAdded)
                            {
                                continue;
                            }

                            _bidirectionalRelationList.Add(new ImportBidirectionalRelation
                            {
                                RelationTypeId = relationshipType.SystemId,
                                Item1Id = newCategory.SystemId,
                                Item2Id = relatedItemId
                            });
                        }

                        if (relationshipLink is CategoryToBaseProductRelationshipLink)
                        {
                            newCategory.RelationshipLinks.Add(
                                new CategoryToBaseProductRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is CategoryToVariantRelationshipLink)
                        {
                            newCategory.RelationshipLinks.Add(
                                new CategoryToVariantRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is CategoryToCategoryRelationshipLink)
                        {
                            newCategory.RelationshipLinks.Add(
                                new CategoryToCategoryRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                    }
                }
            }
        }

        private void AddRelations(StructureInfo structureInfo, Variant variant, Variant newVariant)
        {
            if (variant.RelationshipLinks != null && variant.RelationshipLinks.Count > 0)
            {
                if (newVariant.RelationshipLinks == null)
                {
                    newVariant.RelationshipLinks = new List<VariantToRelationshipLinkBase>();
                }
                foreach (var relationshipLink in variant.RelationshipLinks)
                {
                    var relationshipType = _relationshipTypeService.Get(structureInfo.Id(relationshipLink.RelationshipTypeSystemId));
                    var relatedItemId = Guid.Empty;
                    if (relationshipLink is VariantToBaseProductRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(
                                ((VariantToBaseProductRelationshipLink)relationshipLink).BaseProductSystemId);
                        relatedItemId = newId ==
                                        ((VariantToBaseProductRelationshipLink)relationshipLink).BaseProductSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is VariantToVariantRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((VariantToVariantRelationshipLink)relationshipLink).VariantSystemId);
                        relatedItemId = newId == ((VariantToVariantRelationshipLink)relationshipLink).VariantSystemId
                            ? Guid.Empty
                            : newId;
                    }
                    else if (relationshipLink is VariantToCategoryRelationshipLink)
                    {
                        var newId =
                            structureInfo.Id(((VariantToCategoryRelationshipLink)relationshipLink).CategorySystemId);
                        relatedItemId = newId == ((VariantToCategoryRelationshipLink)relationshipLink).CategorySystemId
                            ? Guid.Empty
                            : newId;
                    }
                    if (relatedItemId != Guid.Empty)
                    {
                        if (relationshipType.Bidirectional)
                        {
                            //Avoid adding duplicates
                            var relationWasAdded =
                                _bidirectionalRelationList.Any(x => x.RelationTypeId == relationshipType.SystemId &&
                                                                    x.Item1Id == relatedItemId &&
                                                                    x.Item2Id == newVariant.SystemId);
                            if (relationWasAdded)
                            {
                                continue;
                            }

                            _bidirectionalRelationList.Add(new ImportBidirectionalRelation
                            {
                                RelationTypeId = relationshipType.SystemId,
                                Item1Id = newVariant.SystemId,
                                Item2Id = relatedItemId
                            });
                        }

                        if (relationshipLink is VariantToBaseProductRelationshipLink)
                        {
                            newVariant.RelationshipLinks.Add(
                                new VariantToBaseProductRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is VariantToVariantRelationshipLink)
                        {
                            newVariant.RelationshipLinks.Add(
                                new VariantToVariantRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                        else if (relationshipLink is VariantToCategoryRelationshipLink)
                        {
                            newVariant.RelationshipLinks.Add(
                                new VariantToCategoryRelationshipLink(relationshipType.SystemId, relatedItemId));
                        }
                    }
                }
            }
        }

        private string GetNewId(StructureInfo structureInfo, string id, Guid systemId)
        {
            //only limit to 100 chars to not break the db constraint
            var newId = $"{structureInfo.Prefix}-{(string.IsNullOrEmpty(id) ? systemId.ToString() : id)}";
            return newId.Length < 100 ? newId : newId.Substring(0, 100);
        }

        /// <summary>
        ///     Imports the categories.
        /// </summary>
        /// <param name="structureInfo">The structure info.</param>
        private void ImportCategories(StructureInfo structureInfo)
        {
            foreach (var category in structureInfo.ProductCatalog.Categories)
            {
                var newCategory = new Category(structureInfo.Id(category.FieldTemplateSystemId),
                    structureInfo.Id(structureInfo.ProductCatalog.Assortment.SystemId))
                {
                    SystemId = structureInfo.Id(category.SystemId),
                    ParentCategorySystemId = structureInfo.Id(category.ParentCategorySystemId),
                    AccessControlList = category.AccessControlList.MakeWritable()
                };
                MapCategories(structureInfo, category.ProductLinks, newCategory.ProductLinks);
                _structureInfoService.AddProperties<ProductArea>(structureInfo, category.Fields, newCategory.Fields, false);
                MapPublish(structureInfo, category.ChannelLinks, newCategory.ChannelLinks);
                AddRelations(structureInfo, category, newCategory);
                _categoryService.Create(newCategory);
            }
        }

        /// <summary>
        ///     Imports the products.
        /// </summary>
        /// <param name="structureInfo">The structure info.</param>
        private void ImportProducts(StructureInfo structureInfo)
        {
            foreach (var product in structureInfo.ProductCatalog.BaseProducts)
            {
                var newSystemId = structureInfo.Id(product.SystemId);
                var newId = GetNewId(structureInfo, product.Id, newSystemId);
                var baseProduct = new BaseProduct(newId, structureInfo.Id(product.FieldTemplateSystemId))
                {
                    SystemId = newSystemId,
                    TaxClassSystemId = structureInfo.Id(product.TaxClassSystemId),
                    AccessControlList = product.AccessControlList.MakeWritable()
                };
                _structureInfoService.AddProperties<ProductArea>(structureInfo, product.Fields, baseProduct.Fields, false);
                foreach (var language in _languageService.GetAll())
                {
                    var url = baseProduct.Localizations[language.CultureInfo].Url;
                    if (string.IsNullOrEmpty(url))
                    {
                        continue;
                    }
                    baseProduct.Localizations[language.CultureInfo].Url = $"{structureInfo.Prefix}_{url}";
                }
                _baseProductService.Create(baseProduct);

                foreach (var variant in structureInfo.ProductCatalog.Variants.Where(x => x.BaseProductSystemId == product.SystemId))
                {
                    newSystemId = structureInfo.Id(variant.SystemId);
                    newId = GetNewId(structureInfo, variant.Id, newSystemId);
                    var newVariant =
                        new Variant(newId, structureInfo.Id(baseProduct.SystemId))
                        {
                            SystemId = newSystemId,
                            UnitOfMeasurementSystemId = variant.UnitOfMeasurementSystemId == null
                                ? null
                                : (Guid?)structureInfo.Id(variant.UnitOfMeasurementSystemId.Value)
                        };
                    _structureInfoService.AddProperties<ProductArea>(structureInfo, variant.Fields, newVariant.Fields);

                    MapPublish(structureInfo, variant.ChannelLinks, newVariant.ChannelLinks);

                    _variantService.Create(newVariant);
                }
                _baseProductService.Update(baseProduct);
            }
            AddPrices(structureInfo);
            AddInventories(structureInfo);
            AddProductLists(structureInfo);

            foreach (var product in structureInfo.ProductCatalog.BaseProducts)
            {
                var newProduct = _baseProductService.Get(structureInfo.Id(product.SystemId)).MakeWritableClone();
                AddRelations(structureInfo, product, newProduct);
                _baseProductService.Update(newProduct);
                foreach (var variant in structureInfo.ProductCatalog.Variants.Where(x => x.BaseProductSystemId == product.SystemId))
                {
                    var newVariant = _variantService.Get(structureInfo.Id(variant.SystemId)).MakeWritableClone();
                    AddBundles(structureInfo, variant, newVariant);
                    AddRelations(structureInfo, variant, newVariant);
                    _variantService.Update(newVariant);
                }
            }

            if (structureInfo.ProductCatalog.ProductFilters == null)
            {
                return;
            }
            var filters = _filterService.GetProductFilteringFields();
            var missingFilters = structureInfo.ProductCatalog.ProductFilters.Where(x => !filters.Contains(x)).ToList();
            if (missingFilters.Any())
            {
                _filterService.SaveProductFilteringFields(missingFilters.Union(filters).ToList());
            }
        }

        private void MapCategories(StructureInfo structureInfo,
            ICollection<CategoryToProductLink> oldCategories, ICollection<CategoryToProductLink> newCategories)
        {
            foreach (var categoryLink in oldCategories)
            {
                var baseProductSystemId = structureInfo.Id(categoryLink.BaseProductSystemId);
                newCategories.Add(new CategoryToProductLink(baseProductSystemId)
                {
                    MainCategory = categoryLink.MainCategory,
                    ActiveVariantSystemIds = new HashSet<Guid>(categoryLink.ActiveVariantSystemIds.Select(structureInfo.Id))
                });
            }
        }

        private void MapPublish(StructureInfo structureInfo, ICollection<VariantToChannelLink> oldChannelLink,
            ICollection<VariantToChannelLink> newChannelLink)
        {
            foreach (var item in oldChannelLink)
            {
                if (item.ChannelSystemId == structureInfo.Website.Channel.SystemId)
                {
                    newChannelLink.Add(new VariantToChannelLink(structureInfo.Id(item.ChannelSystemId)));
                }
            }
        }

        private void MapPublish(StructureInfo structureInfo, ICollection<CategoryToChannelLink> oldChannelLink,
            ICollection<CategoryToChannelLink> newChannelLink)
        {
            foreach (var item in oldChannelLink)
            {
                if (item.ChannelSystemId == structureInfo.Website.Channel.SystemId)
                {
                    newChannelLink.Add(new CategoryToChannelLink(structureInfo.Id(item.ChannelSystemId)));
                }
            }
        }

        private class ImportBidirectionalRelation
        {
            public Guid Item1Id { set; get; }
            public Guid Item2Id { set; get; }
            public Guid RelationTypeId { set; get; }
        }
    }
}

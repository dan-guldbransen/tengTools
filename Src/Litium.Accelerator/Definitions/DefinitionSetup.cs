using System;
using System.Collections.Generic;
using Litium.Accelerator.Search.Filtering;
using Litium.Accelerator.Services;
using Litium.Common;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.FieldFramework.Internal;
using Litium.Owin.Lifecycle;
using Litium.Products;
using Litium.Security;
using Microsoft.Extensions.Logging;
using Block = Litium.Blocks;

namespace Litium.Accelerator.Definitions
{
    public class DefinitionSetup : IStartupTask
    {
        private readonly FieldTemplateService _fieldTemplateService;
        private readonly SettingService _settingService;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly SecurityContextService _securityContextService;
        private readonly RelationshipTypeService _relationshipTypeService;
        private readonly DisplayTemplateService _displayTemplateService;
        private readonly Block.CategoryService _categoryService;
        private readonly AddressTypeService _addressTypeService;
        private readonly RoleService _roleService;
        private readonly TemplateSettingService _templateSettingService;
        private readonly IEnumerable<FieldDefinitionSetup> _definitionSetups;
        private readonly IEnumerable<DisplayTemplateSetup> _displayTemplateSetups;
        private readonly IEnumerable<FieldTemplateSetup> _fieldTemplateSetups;
        private readonly IEnumerable<RelationshipTypeSetup> _relationshipTypeSetups;
        private readonly IEnumerable<BlockCategorySetup> _categorySetups;
        private readonly IEnumerable<AddressTypeSetup> _addressTypeSetups;
        private readonly IEnumerable<RoleSetup> _roleSetups;
        private readonly FieldFrameworkSetupLocalizationService _fieldFrameworkSetupLocalizationService;
        private readonly FilterService _filterService;
        private readonly ILogger<DefinitionSetup> _logger;
        public static bool DefinitionsError { get; private set; }

        public DefinitionSetup(
            FieldDefinitionService fieldDefinitionService,
            FieldTemplateService fieldTemplateService,
            SettingService settingService,
            SecurityContextService securityContextService,
            RelationshipTypeService relationshipTypeService,
            DisplayTemplateService displayTemplateService,
            Block.CategoryService categoryService,
            AddressTypeService addressTypeService,
            RoleService roleService,
            TemplateSettingService templateSettingService,
            IEnumerable<FieldDefinitionSetup> fieldDefinitonSetups,
            IEnumerable<FieldTemplateSetup> fieldTemplateSetups,
            IEnumerable<DisplayTemplateSetup> displayTemplateSetups,
            IEnumerable<RelationshipTypeSetup> relationshipTypeSetups, 
            IEnumerable<BlockCategorySetup> categorySetup, 
            IEnumerable<AddressTypeSetup> addressTypeSetup, 
            IEnumerable<RoleSetup> roleSetups,
            FieldFrameworkSetupLocalizationService fieldFrameworkSetupLocalizationService, 
            FilterService filterService, ILogger<DefinitionSetup> logger)
        {
            _fieldDefinitionService = fieldDefinitionService;
            _fieldTemplateService = fieldTemplateService;
            _settingService = settingService;
            _definitionSetups = fieldDefinitonSetups;
            _fieldTemplateSetups = fieldTemplateSetups;
            _securityContextService = securityContextService;
            _relationshipTypeService = relationshipTypeService;
            _displayTemplateService = displayTemplateService;
            _categoryService = categoryService;
            _templateSettingService = templateSettingService;
            _displayTemplateSetups = displayTemplateSetups;
            _relationshipTypeSetups = relationshipTypeSetups;
            _categorySetups = categorySetup;
            _addressTypeService = addressTypeService;
            _addressTypeSetups = addressTypeSetup;
            _roleSetups = roleSetups;
            _roleService = roleService;
            _fieldFrameworkSetupLocalizationService = fieldFrameworkSetupLocalizationService;
            _filterService = filterService;
            _logger = logger;
        }

        public void Start()
        {
            using (_securityContextService.ActAsSystem("setup"))
            {
                foreach (var item in _definitionSetups)
                {
                    InitFields(item.GetFieldDefinitions());
                }

                foreach (var item in _displayTemplateSetups)
                {
                    InitDisplayTemplates(item.GetDisplayTemplates());
                }

                foreach (var item in _relationshipTypeSetups)
                {
                    InitRelationshipTypes(item.GetRelationshipTypes());
                }

                foreach (var item in _addressTypeSetups)
                {
                    InitAddressType(item.GetAddressTypes());
                }

                foreach (var roleSetup in _roleSetups)
                {
                    InitRoles(roleSetup.GetRoles());
                }

                foreach (var categorySetup in _categorySetups)
                {
                    InitCategories(categorySetup.GetCategories());
                }

                foreach (var item in _fieldTemplateSetups)
                {
                    InitTemplates(item.GetTemplates());
                }

                if (!IsAlreadyExecuted<FieldTemplate>("ProductWithVariants:Settings"))
                {
                    _templateSettingService.SetTemplateGroupings("ProductWithVariants", "color");
                    SetAlreadyExecuted<FieldTemplate>("ProductWithVariants:Settings");
                }

                if (!IsAlreadyExecuted<FieldTemplate>("ProductFilters:Settings"))
                {
                    var filters = new List<String>
                    {
                        "Size",
                        "#Price",
                        "Color",
                        "Brand",
                        "#News"
                    };
                    _filterService.SaveProductFilteringFields(filters);
                    SetAlreadyExecuted<FieldTemplate>("ProductFilters:Settings");
                }
            }
        }

        private void InitRoles(IEnumerable<Role> roles)
        {
            foreach (var item in roles)
            {
                if (IsAlreadyExecuted<Role>(item.Id))
                {
                    continue;
                }
                var currentField = _roleService.Get(item.Id);
                if (currentField != null)
                {
                    continue;
                }
                _roleService.Create(item);
                SetAlreadyExecuted<Role>(item.Id);
            }
        }

        private void InitCategories(IEnumerable<Block.Category> categories)
        {
            foreach (var category in categories)
            {
                if (IsAlreadyExecuted<Category>(category.Id))
                {
                    continue;
                }
                var currentCategory = _categoryService.Get(category.Id);
                if (currentCategory != null)
                {
                    continue;
                }
                _categoryService.Create(category);
                SetAlreadyExecuted<Category>(category.Id);
            }
        }

        private void InitDisplayTemplates(IEnumerable<DisplayTemplate> displayTemplates)
        {
            foreach (var displayTemplate in displayTemplates)
            {
                if (IsAlreadyExecuted<DisplayTemplate>(displayTemplate.Id))
                {
                    continue;
                }
                var currentField = _displayTemplateService.Get<DisplayTemplate>(displayTemplate.Id);
                if (currentField != null)
                {
                    continue;
                }
                _displayTemplateService.Create(displayTemplate);
                SetAlreadyExecuted<DisplayTemplate>(displayTemplate.Id);
            }
        }

        private void InitFields(IEnumerable<FieldDefinition> fields)
        {
            foreach (var item in fields)
            {
                if (IsAlreadyExecuted<FieldDefinition>(item.Id, item.AreaType.Name))
                {
                    //continue;
                }

                var currentField = _fieldDefinitionService.Get(item.AreaType, item.Id);
                if (currentField == null)
                {
                    _fieldFrameworkSetupLocalizationService.Localize(item);
                    _fieldDefinitionService.Create(item);
                    SetAlreadyExecuted<FieldDefinition>(item.Id, item.AreaType.Name);
                    continue;
                }

                if (item.FieldType != currentField.FieldType)
                {
                    _logger.LogError($"Accelerator \"{item.Id}\" field with \"{item.FieldType}\" type can't be created. The system already has the \"{currentField.Id}\" field with \"{currentField.FieldType}\" type. Accelerator deployment would fail.");
                    DefinitionsError = true;
                    continue;
                }
                if (item.MultiCulture != currentField.MultiCulture)
                {
                    _logger.LogError($"Accelerator \"{item.Id}\" field with \"MultiCulture\" setting and \"{item.MultiCulture}\" value can't be created. The system already has the \"{currentField.Id}\" field with \"MultiCulture\" setting and \"{currentField.MultiCulture}\" value. Accelerator deployment would fail.");
                    DefinitionsError = true;
                    continue;
                }

                SetAlreadyExecuted<FieldDefinition>(item.Id, item.AreaType.Name);
            }
        }

        private void InitRelationshipTypes(IEnumerable<RelationshipType> relationshipTypes)
        {
            foreach (var item in relationshipTypes)
            {
                if (IsAlreadyExecuted<RelationshipType>(item.Id))
                {
                    continue;
                }
                var currentItem = _relationshipTypeService.Get(item.Id);
                if (currentItem != null)
                {
                    continue;
                }
                _relationshipTypeService.Create(item);
                SetAlreadyExecuted<RelationshipType>(item.Id);
            }
        }

        private void InitAddressType(IEnumerable<AddressType> addressTypes)
        {
            foreach (var type in addressTypes)
            {
                if (IsAlreadyExecuted<AddressType>(type.Id)) 
                {
                    continue;
                }
                var currentField = _addressTypeService.Get(type.Id);
                if (currentField != null)
                {
                    continue;
                }
                _addressTypeService.Create(type);
                SetAlreadyExecuted<AddressType>(type.Id);
            }
        }

        private void InitTemplates(IEnumerable<FieldTemplate> templates)
        {
            foreach (var item in templates)
            {
                if (IsAlreadyExecuted<FieldTemplate>(item.Id, item.AreaType.Name))
                {
                    //continue;
                }

                var currentField = _fieldTemplateService.Get<FieldTemplate>(item.AreaType, item.Id);
                try
                {
                    _fieldFrameworkSetupLocalizationService.Localize(item, ((dynamic)item).FieldGroups as ICollection<FieldTemplateFieldGroup>);
                }
                catch
                {
                    _fieldFrameworkSetupLocalizationService.Localize(item);
                }

                if (currentField == null)
                {
                    _fieldTemplateService.Create(item);
                }
                else
                {
                    item.SystemId = currentField.SystemId;
                    _fieldTemplateService.Update(item);
                }

                SetAlreadyExecuted<FieldTemplate>(item.Id, item.AreaType.Name);
            }
        }

        private bool IsAlreadyExecuted<T>(string id, string area = null)
        {
            if (area == null)
            {
                return _settingService.Get<bool>($"AcceleratorBuilder:{typeof(T).FullName}:{id}");
            }
            return _settingService.Get<bool>($"AcceleratorBuilder:{typeof(T).FullName}:{area}:{id}");
        }

        private void SetAlreadyExecuted<T>(string id, string area = null)
        {
            if (area == null)
            {
                _settingService.Set($"AcceleratorBuilder:{typeof(T).FullName}:{id}", true);
            }

            _settingService.Set($"AcceleratorBuilder:{typeof(T).FullName}:{area}:{id}", true);
        }
    }
}

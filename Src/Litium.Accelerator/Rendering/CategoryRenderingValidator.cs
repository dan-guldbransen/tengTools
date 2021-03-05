using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Utilities;
using Litium.FieldFramework.FieldTypes;
using Litium.Products;
using Litium.Web.Rendering;

namespace Litium.Accelerator.Rendering
{
    internal class CategoryRenderingValidator : IRenderingValidator<Category>
    {
        private readonly PersonStorage _personStorage;

        public CategoryRenderingValidator(PersonStorage personStorage)
        {
            _personStorage = personStorage;
        }

        public bool Validate(Category model)
        {
            var organizations = model.Fields.GetValue<IList<PointerItem>>(ProductFieldNameConstants.OrganizationsPointer)?.Select(x => x.EntitySystemId).ToHashSet();
            if (organizations == null
                || (_personStorage.CurrentSelectedOrganization != null && organizations.Contains(_personStorage.CurrentSelectedOrganization.SystemId)))
            {
                return true;
            }

            return false;
        }
    }
}

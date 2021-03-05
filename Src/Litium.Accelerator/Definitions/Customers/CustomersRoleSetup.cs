using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.Customers;

namespace Litium.Accelerator.Definitions.Customers
{
    internal class CustomersRoleSetup : RoleSetup
    {
        public override IEnumerable<Role> GetRoles()
        {
            var items = new List<Role>
            {
                new Role()
                {
                    Id = RolesConstants.RoleOrderApprover,
                    Localizations =
                    {
                        ["sv-SE"] = { Name = RolesConstants.RoleOrderApprover },
                        ["en-US"] = { Name = RolesConstants.RoleOrderApprover },
                    }
                },
                new Role()
                {
                    Id = RolesConstants.RoleOrderPlacer,
                    Localizations =
                    {
                        ["sv-SE"] = { Name = RolesConstants.RoleOrderPlacer },
                        ["en-US"] = { Name = RolesConstants.RoleOrderPlacer },
                    }
                }
            };
            return items;
        }
    }
}

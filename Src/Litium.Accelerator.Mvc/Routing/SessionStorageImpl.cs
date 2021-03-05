using System.Web;
using Litium.Accelerator.Utilities;
using Litium.Customers;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Security;

namespace Litium.Accelerator.Mvc.Routing
{
    public class SessionStorageImpl : SessionStorage
    {
        public override T GetValue<T>(string name)
        {
            var item = HttpContext.Current?.Session?[name];
            if (item is T value)
            {
                return value;
            }
            return default;
        }

        public override void SetValue<T>(string name, T value)
        {
            HttpContext.Current.Session[name] = value;
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;
using System.Web.Mvc;

namespace Litium.Accelerator.Mvc.Extensions
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// Serialize the object to string using Camel case resolver.
        /// </summary>
        /// <param name="htmlHelper">The html helper.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The IHtmlString which represents the serialized object.</returns>
        public static IHtmlString Json(this HtmlHelper htmlHelper, object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return new HtmlString(JsonConvert.SerializeObject(obj, settings));
        }
    }
}

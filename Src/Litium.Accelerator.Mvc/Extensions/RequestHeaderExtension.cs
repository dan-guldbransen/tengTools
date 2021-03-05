using Litium.Accelerator.ViewModels.Framework;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http.Headers;

namespace Litium.Accelerator.Mvc.Extensions
{
    public static class RequestHeaderExtension
    {
        public static SiteSettingViewModel GetSiteSettingViewModel(this HttpRequestHeaders httpHeaders)
        {
            if (httpHeaders.TryGetValues("litium-request-context", out var items))
            {
                var json = items.First();
                var jObject = JObject.Parse(json);
                return jObject.ToObject<SiteSettingViewModel>();
            }
            return null;
        }
    }
}
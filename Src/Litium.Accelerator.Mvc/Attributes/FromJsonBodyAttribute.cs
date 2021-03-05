using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Litium.Accelerator.Mvc.Attributes
{
    public class FromJsonBodyAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }

        private class JsonModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var stream = controllerContext.HttpContext.Request.InputStream;
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    var checkoutOrderDataStr = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject(checkoutOrderDataStr, bindingContext.ModelType);
                }
            }
        }
    }
}
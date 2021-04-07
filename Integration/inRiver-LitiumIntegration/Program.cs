using AutoMapper;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using inRiver_LitiumIntegration.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration
{
    class Program 
    {
        static void Main(string[] args)
        {
            const string InRiverRemotingUrl = "https://remoting.productmarketingcloud.com";
            const string InRiverUsername = "inriver-tengtools@consid.se";
            const string InRiverPassword = "544%%IdkwDWHk\"XgbeU3pdD"; //notera escapat " i lösen... Autogenerat :)
            const string InRiverEnvironmentProd = "prod";
            const string InRiverEnvironmentTest = "test";

            const string ProductTypeId = "Product";
            const string ItemTypeId = "Item";
            const string ResourceTypeId = "Resource";


            Console.WriteLine("Connecting...");
            var context = new inRiverContext(RemoteManager.CreateInstance(InRiverRemotingUrl, InRiverUsername, InRiverPassword, InRiverEnvironmentTest), new ConsoleLogger());
            Console.WriteLine("Connected!");

            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ProductTypeId, LoadLevel.DataAndLinks);
            var root = new Connect_classes.Root();

            foreach (var item in products)
            {

                if (item.EntityType.Id == "Product")
                {
                    var config = new MapperConfiguration(cfg => { cfg.CreateMap<List<Field>, List<Connect_classes.Field>>(); });
                    IMapper iMapper = config.CreateMapper();
                    var fields = iMapper.Map<List<Field>, List<Connect_classes.Field>>(products[0].Fields);
                    var sourceProduct = products.First();
                    var targetProduct = new Connect_classes.Product();
                    root.products = new List<Connect_classes.Product>();
                    targetProduct.fields = fields;
                    targetProduct.articleNumber = ""; //sourceProduct.GetField("articleNubmer")?.Data.ToString();
                    targetProduct.fieldTemplateId = "ProductBrand"; // sourceProduct.fi .GetField("FieldType.Id")?.Data.ToString();
                    targetProduct.taxClassId = ""; // sourceProduct.GetField("articleNubmer")?.Data.ToString();
                    foreach (var f in sourceProduct.Fields)
                    {
                        var field = new Connect_classes.Field(f);
                        targetProduct.fields.Add(field);
                    }

                    root.products.Add(targetProduct);
                    root.importBehavior = "stopOnAnyError";
                }
                else if (item.EntityType.Id == "Item")
                {

                }
                else if (item.EntityType.Id == "Resource")
                {

                }
            }


            using (var client = new HttpClient()) 
            {
                string json = JsonConvert.SerializeObject(root);
                client.BaseAddress = new Uri("http://localhost:51134/");
                client.DefaultRequestHeaders.Add("api-version","2.0");
                client.DefaultRequestHeaders.Add("ContentType","application/json");
                var response = client.PostAsJsonAsync("/Litium/api/connect/erp/imports?api-version=2.0", json).Result; 

                Console.WriteLine(response);
            }

            Console.ReadKey();

           

           
   
}

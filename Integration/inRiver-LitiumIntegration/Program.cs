using AutoMapper;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using inRiver_LitiumIntegration.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
            //var context = new inRiverContext(RemoteManager.CreateInstance(InRiverRemotingUrl, InRiverUsername, InRiverPassword, InRiverEnvironmentProd), new ConsoleLogger());
            Console.WriteLine("Connected!");

            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0,ItemTypeId, LoadLevel.DataAndLinks);
            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ProductTypeId, LoadLevel.DataAndLinks);
            var resources = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ResourceTypeId, LoadLevel.DataAndLinks);
            var root = new Connect_classes.Root();
            root.importBehavior = "stopOnAnyError";
            root.variants = new List<Connect_classes.Variant>();
            root.products = new List<Connect_classes.Product>();

            foreach (var workingProduct in products)
            {
                var targetProduct = new Connect_classes.Product();
                var sourceProduct = workingProduct;
                var multipleVariants = workingProduct.OutboundLinks.Count() > 1 ? true : false;
               
                targetProduct.articleNumber = workingProduct.GetField("ProductID").Data?.ToString() ?? "";
                targetProduct.fieldTemplateId = multipleVariants ? "ProductWithVariants" : "ProductWithOneVariant";
                targetProduct.taxClassId = "";
                foreach (var f in sourceProduct.Fields)
                {
                    var field = new Connect_classes.Field(f);
                    targetProduct.fields.Add(field);
                }

                root.products.Add(targetProduct);
            }

            foreach (var workingItem in items)
            {
                var targetVariant = new Connect_classes.Variant();
                targetVariant.fields = new List<Connect_classes.Field>();
                targetVariant.articleNumber = workingItem.GetField("ItemId").Data.ToString();
                targetVariant.productArticleNumber = workingItem.InboundLinks.FirstOrDefault().Id.ToString();
                targetVariant.sortIndex  = 0;
                targetVariant.unitOfMeasurementId = "";

                foreach (var f in workingItem.Fields)
                {
                    var field = new Connect_classes.Field(f);
                    targetVariant.fields.Add(field);
                }

                root.variants.Add(targetVariant);
            }

            

            foreach (var workingResource in resources)
            {
                //var targetResource = new Connect_classes.  .Variant();
            }

            string jwtToken = GetJWTToken();

            //Byt ut porten till den du kör på
            var restClient = new RestClient("http://localhost:8080/Litium/api/connect/erp/imports?api-version=2.0");
            restClient.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + jwtToken);
            request.AddHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(root);
            request.AddParameter(json, ParameterType.RequestBody);
            IRestResponse response = restClient.Execute(request);

            Console.WriteLine(response.Content);
            Console.ReadKey();
        }

        static string GetJWTToken()
        {
            //Byt ut porten till den du kör på
            var restClient = new RestClient("http://localhost:8080/Litium/oauth/token");
            restClient.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            //Skapa ett servicekonto med samma uppgifter eller byt ut till ditt konto
            request.AddParameter("client_id", "serviceID");
            request.AddParameter("client_secret", "servicepw");
            IRestResponse restResponse = restClient.Execute(request);
            var jObject = JObject.Parse(restResponse.Content);
            string access_token = jObject.GetValue("access_token").ToString();

            return access_token;
        }
    }
           
   
}

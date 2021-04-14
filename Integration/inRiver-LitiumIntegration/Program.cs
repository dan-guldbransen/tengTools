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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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

            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ItemTypeId, LoadLevel.DataAndLinks);
            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ProductTypeId, LoadLevel.DataAndLinks);
            var resources = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ResourceTypeId, LoadLevel.DataAndLinks);
            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues();
            var selectedVal3 = cvlValues.Where(v => v.CVLId == "Currency");


            var root = new Connect_classes.Root();
            root.importBehavior = "stopOnAnyError";
            root.variants = new List<Connect_classes.Variant>();
            root.products = new List<Connect_classes.Product>();

            foreach (var workingProduct in products)
            {
                var targetProduct = new Connect_classes.Product();
                var sourceProduct = workingProduct;
                var multipleVariants = workingProduct.OutboundLinks.Count() > 1 ? true : false;

                targetProduct.articleNumber = workingProduct.GetField("ProductID").Data?.ToString() ?? "test_" + workingProduct.Id.ToString() + "";  // nullkontroll har testId
                targetProduct.fieldTemplateId = multipleVariants ? "ProductWithVariants" : "ProductWithOneVariant";
                targetProduct.taxClassId = null;
              
                foreach (var f in sourceProduct.Fields)
                {
                    if(f.FieldType.DataType == "CVL")
                    {
                        f.Data = GetCvlDisplayValues(cvlValues, f);
                    }
                    var field = new Connect_classes.Field(f);
                    targetProduct.fields.Add(field);
                }
                if (workingProduct == products[5] || workingProduct == products[6] || workingProduct == products[7])
                {
                root.products.Add(targetProduct);

                }
            }
            root.productSettings = new Connect_classes.ProductSettings();
            root.productSettings.createUrls = false;
            root.productSettings.fieldTemplateId = "ProductWithVariants";
            
            foreach (var workingItem in items)
            {
                var targetVariant = new Connect_classes.Variant();
                targetVariant.fields = new List<Connect_classes.Field>();
                targetVariant.articleNumber = workingItem.GetField("ItemId").Data.ToString();
                targetVariant.productArticleNumber = workingItem.InboundLinks.FirstOrDefault().Id.ToString();
                targetVariant.sortIndex = 0;
                targetVariant.unitOfMeasurementId = "";

                foreach (var f in workingItem.Fields)
                {
                    if (f.FieldType.CategoryId != "PriceAdjustmentInfo")
                    { 

                        if (f.FieldType.DataType == "CVL")
                        {
                            f.Data = GetCvlDisplayValues(cvlValues, f);
                        }
                        var field = new Connect_classes.Field(f);
                        targetVariant.fields.Add(field);
                    }

                }
               // root.variants.Add(targetVariant);
            }

            root.variantSettings = new Connect_classes.VariantSettings();
            root.variantSettings.createBaseProducts = true;
            root.variantSettings.createUrls = true;

            foreach (var workingResource in resources)
            {
                //var targetResource = new Connect_classes.  .Variant();
            }

            string jwtToken = GetJWTToken();

            //Byt ut porten till den du kör på

          //  var restClient = new RestClient("http://localhost:51134/Litium/api/connect/erp/imports?api-version=2.0");
            var restClient = new RestClient("http://tengtools.localtest.me:8050/Litium/api/connect/erp/imports?api-version=2.0");
            restClient.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + jwtToken);
            request.AddHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(root);
            /*
           string json = JsonConvert.SerializeObject(root, Formatting.None,
            new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            */ 
            request.AddParameter(json, ParameterType.RequestBody);
            IRestResponse response = restClient.Execute(request);

            Console.WriteLine(response.Content);
            Console.ReadKey();
        }

        static string GetJWTToken()
        {
            //Byt ut porten till den du kör på
           // var restClient = new RestClient("http://localhost:51134/Litium/oauth/token");
            var restClient = new RestClient("http://tengtools.localtest.me:8050/Litium/oauth/token");
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
      
       // static CvlForExport GetCvlDisplayValues( List<CVLValue> cvlValues,  Field f)
        static List<string> GetCvlDisplayValues( List<CVLValue> cvlValues,  Field f)
        {
            var output = new CvlForExport();
            var testList = new List<string>();
            var tempValues = cvlValues.Where(v => v.CVLId == f.FieldType.CVLId);

            foreach (var value in tempValues)
            {
                var sb = new StringBuilder();
                sb.Append("MultiSelect: ");
                sb.Append(f.FieldType.Multivalue);
                testList.Add(sb.ToString());
                 sb.Clear();
                sb.Append("Value: ");
                var val = new CvlForExport();
                testList.Add(sb.ToString());
                if (value.Value != null)
                {
                     sb.Clear();
                    var testOptionVal = new CvlForExport.Item();
                    testOptionVal.Value = value.Key;
                    testOptionVal.Name = value.Value.ToString();
                    sb.Append("Value: ");
                    sb.Append(value.Key);
                    testList.Add(sb.ToString());
                    sb.Clear();
                    sb.Append("Name: ");
                    sb.Append(value.Value.ToString());
                    testList.Add(sb.ToString());
                    output.Items.Add(testOptionVal);
                    
                }
            }
            output.MultiSelect = f.FieldType.Multivalue;
            return testList;
            //return output;
        }
     
    }
    class CvlForExport
    {
        public IList<Item> Items { get; set; }
        public bool MultiSelect { get; set; }
        public bool ManualSort { get; set; }
        public CvlForExport()
        {
            ManualSort = true;
            Items = new List<Item>();
        }
        public class Item 
        {
           // public IDictionary<string, string> Name { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
           
   
}

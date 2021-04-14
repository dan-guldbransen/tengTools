using inRiver.DataSyncTask.Models;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask
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
            const string CategoryTypeId = "Category";
            const string ProductItemLink = "ProductItem";

            Console.WriteLine("Connecting...");
            var context = new inRiverContext(RemoteManager.CreateInstance(InRiverRemotingUrl, InRiverUsername, InRiverPassword, InRiverEnvironmentTest), new ConsoleLogger());
            Console.WriteLine("Connected!");

            var categories = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, CategoryTypeId, LoadLevel.DataAndLinks);
            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ProductTypeId, LoadLevel.DataAndLinks);
            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, ItemTypeId, LoadLevel.DataAndLinks);

            var data = new ProductData();

            var product = products.First();
            var multipleVariants = product.OutboundLinks.Where(l => l.LinkType.Id == ProductItemLink).Count() > 1 ? true : false;

            var targetProduct = new Product();

            targetProduct.ArticleNumber = product.GetField("ProductID").Data?.ToString() ?? "test_" + product.Id.ToString() + "";  // nullkontroll har testId
            targetProduct.FieldTemplateId = multipleVariants ? "ProductWithVariants" : "ProductWithOneVariant";
            targetProduct.TaxClassId = null;
            targetProduct.Fields.Add(new Models.Field
            {
                FieldDefinitionId = "_name",
                Value = "Produktnamn",
                Culture = "sv-SE"
            });
         
            targetProduct.Fields.Add(new Models.Field
            {
                FieldDefinitionId = "_name",
                Value = "Produktnamn US",
                Culture = "en-US"
            });

            targetProduct.Fields.Add(new Models.Field
            {
                FieldDefinitionId = "_description",
                Value = "Beskrivniiiing US",
                Culture = "en-US"
            });

            data.Products.Add(targetProduct);
            var link = product.OutboundLinks.FirstOrDefault(l => l.LinkType.Id == ProductItemLink);
            if(link != null)
            {
                var variant = items.FirstOrDefault(x => x.Id == link.Target.Id);
                data.Variants.Add(new Variant
                {
                    ProductArticleNumber = targetProduct.ArticleNumber,
                    ArticleNumber = variant.Id.ToString(),
                    Fields = new List<Models.Field>
                    {
                        new Models.Field
                        {
                            FieldDefinitionId = "_name",
                            Value = "TestVariant",
                            Culture = "sv-SE"
                        },
                        new Models.Field
                        {
                            FieldDefinitionId = "_name",
                            Value = "TestVariant US",
                            Culture = "en-Us"
                        },
                        new Models.Field
                        {
                            FieldDefinitionId = "_description",
                            Value = "Beskrivning US",
                            Culture = "en-Us"
                        }
                    }
                });
            }

            string jwtToken = GetJWTToken().Result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:56020");
                // Request headers for auth
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(
                  json,
                  Encoding.UTF8,
                  "application/json"
                  );

                var result = client.PostAsync("litium/api/connect/erp/imports", content).Result;

                result.EnsureSuccessStatusCode();
            }

        }

        static async Task<string> GetJWTToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:56020");

                var request = new HttpRequestMessage(HttpMethod.Post, "/litium/oauth/token");

                var keyValues = new List<KeyValuePair<string,string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                keyValues.Add(new KeyValuePair<string, string>("client_id", "IntegrationAccount"));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", "consid12345"));

                request.Content = new FormUrlEncodedContent(keyValues);
                var response = await client.SendAsync(request);

                var data = JsonConvert.DeserializeObject<JWTResponse>(response.Content.ReadAsStringAsync().Result);
                return data.AccessToken;
            }
        }
    }
}

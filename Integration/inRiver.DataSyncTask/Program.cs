using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.Litium;
using inRiver.DataSyncTask.Services;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var context = new inRiverContext(RemoteManager.CreateInstance(InRiver.Remoting.InRiverRemotingUrl, InRiver.Remoting.InRiverUsername, InRiver.Remoting.InRiverPassword, InRiver.Remoting.InRiverEnvironmentTest), new ConsoleLogger());
            Console.WriteLine("Connected!");

            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ProductTypeId, LoadLevel.DataAndLinks);
            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ItemTypeId, LoadLevel.DataAndLinks);
            
            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues().ToList();

            //Get inRiver Categories lvl 1 and 2
            var productCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.Category).ToList();
            var productGroups = cvlValues.Where(c => c.CVLId == InRiver.CVL.ProductGroup).ToList();

            var cultures = LitiumCommonService.GetCulturesInUse();

            // Container for all data to post
            // We must do this in batches later
            var data = new Data();

            // Categories (existing categories may be redundant, will check how save data behaves) 
            (string assortmentId, List<string> existingCategorys) = CategoryService.GetAssortmentIdAndExistingCategoryIds();
            
            var categories = CategoryService.ProcessCategoryCVLs(productCategories, productGroups);

            // Resources ??

                var keyValues = new List<KeyValuePair<string,string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                keyValues.Add(new KeyValuePair<string, string>("client_id", "IntegrationAccount"));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", "consid12345"));

                request.Content = new FormUrlEncodedContent(keyValues);
                var response = await client.SendAsync(request);

            // Save data to Litium
            LitiumCommonService.SaveData(data);
        }
    }
}

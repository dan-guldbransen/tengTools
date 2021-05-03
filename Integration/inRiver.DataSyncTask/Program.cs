using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.Litium;
using inRiver.DataSyncTask.Services;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace inRiver.DataSyncTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");
            var context = new inRiverContext(RemoteManager.CreateInstance(InRiver.Remoting.InRiverRemotingUrl, InRiver.Remoting.InRiverUsername, InRiver.Remoting.InRiverPassword, InRiver.Remoting.InRiverEnvironmentTest), new ConsoleLogger());
            Console.WriteLine("Connected!");


            // Get products and items from inRiver
            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ProductTypeId, LoadLevel.DataAndLinks);
            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ItemTypeId, LoadLevel.DataAndLinks);
            
            // Get e-com channel structure for catgories
            var catgeoryStructure = ChannelBuilder.GetChannelStructure(context, products);

            // Get CVL values from inRiver e.g. Categories
            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues().ToList();

            // Get inRiver Categories lvl 2 and 3
            var productCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.Category).ToList();
            var productGroups = cvlValues.Where(c => c.CVLId == InRiver.CVL.ProductGroup).ToList();

            // Get cultures from Litium, we only sync languages we use
            var cultures = LitiumCommonService.GetCulturesInUse();
            
            // Catgeories last so we get the hierarchy of top level from product
            var categoryTemplateSystemId = LitiumCommonService.GetCategoryTemplateSystemId();
            CategoryService.ProcessCategoryCVLs(catgeoryStructure, productCategories, productGroups, cultures, categoryTemplateSystemId);
            
            // Container for all data to post
            var data = new Data();

            // Products
            ProductService.ProcessProducts(products, data, cultures);
           
            // Variants
            VariantService.ProcessVariants(items, data, cultures);
            
            // Resources TODO.

            // Save data to Litium TODO: in batches!!
            var importReportId = LitiumCommonService.SaveData(data);
        }
    }
}

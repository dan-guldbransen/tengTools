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

            var products = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ProductTypeId, LoadLevel.DataAndLinks);
            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ItemTypeId, LoadLevel.DataAndLinks);
            
            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues().ToList();

            // Get inRiver Categories lvl 1 and 2
            var productCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.Category).ToList();
            var productGroups = cvlValues.Where(c => c.CVLId == InRiver.CVL.ProductGroup).ToList();

            var cultures = LitiumCommonService.GetCulturesInUse();

            // Container for all data to post
            // We must do this in batches later
            var data = new Data();

            // Categories (existing categories may be redundant, will check how save data behaves) 
            (string assortmentId, List<Models.LitiumEntities.CategoryEntity> existingCategorys) = CategoryService.GetAssortmentIdAndExistingCategoryIds();
            
            var categories = CategoryService.ProcessCategoryCVLs(productCategories, productGroups, assortmentId, cultures, existingCategorys);

            // Resources ??

            // Products
            ProductService.ProcessProducts(products, data, cultures, categories);
           
            // Variants
            VariantService.ProcessVariants(items, data, cultures);

            // Save data to Litium
            LitiumCommonService.SaveData(data);
        }
    }
}

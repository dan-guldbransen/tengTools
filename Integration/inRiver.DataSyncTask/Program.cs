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
            
            // Get CVL values from inRiver e.g. Categories
            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues().ToList();

            // Get main categories
            var productHeadCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.HeadCategory).ToList();

            // Get inRiver Categories lvl 2 and 3
            var productCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.Category).ToList();
            var productGroups = cvlValues.Where(c => c.CVLId == InRiver.CVL.ProductGroup).ToList();

            // Get cultures from Litium, we only sync languages we use
            var cultures = LitiumCommonService.GetCulturesInUse();
            
            // Container for all data to post
            var data = new Data();
            
            // Key is head category id and values are a list of productcategories that should exist beneath
            var headCategoryHierarchy = new Dictionary<string, List<string>>();

            // Products
            ProductService.ProcessProducts(products, data, cultures, headCategoryHierarchy);
           
            // Variants
            VariantService.ProcessVariants(items, data, cultures);
            
            // Resources here or on product/variant ??
            
            // Catgeories last so we get the hierarchy of top level from product
            var categoryTemplateSystemId = LitiumCommonService.GetCategoryTemplateSystemId();

            // Categories (existing categories may be redundant, will check how save data behaves) 
            (string assortmentId, List<Models.LitiumEntities.CategoryEntity> existingCategorys) = CategoryService.GetAssortmentIdAndExistingCategoryIds();
            
            var categories = CategoryService.ProcessCategoryCVLs(productHeadCategories, productCategories, productGroups, assortmentId, cultures, existingCategorys);

            // Save data to Litium TODO: in batches!!
            LitiumCommonService.SaveData(data);
        }
    }
}

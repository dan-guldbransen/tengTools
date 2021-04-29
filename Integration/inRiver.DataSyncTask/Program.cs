﻿using inRiver.DataSyncTask.Constants;
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
            Console.WriteLine("Connecting...");
            var context = LitiumCommonService.GetContext();
             context = new inRiverContext(RemoteManager.CreateInstance(InRiver.Remoting.InRiverRemotingUrl, InRiver.Remoting.InRiverUsername, InRiver.Remoting.InRiverPassword, InRiver.Remoting.InRiverEnvironmentTest), new ConsoleLogger());
            Console.WriteLine("Connected!");

            var products = LitiumCommonService.GetProducts();
            var items = context.ExtensionManager.DataService.GetEntitiesForEntityType(0, InRiver.EntityType.ItemTypeId, LoadLevel.DataAndLinks);

            var cvlValues = context.ExtensionManager.ModelService.GetAllCVLValues().ToList();

            //Get inRiver Categories lvl 1 and 2  
            var headCategory = cvlValues.Where(c => c.CVLId == InRiver.CVL.HeadCategory).ToList();
            var productCategories = cvlValues.Where(c => c.CVLId == InRiver.CVL.Category).ToList();
            var productGroups = cvlValues.Where(c => c.CVLId == InRiver.CVL.ProductGroup).ToList();

            var cultures = LitiumCommonService.GetCulturesInUse();

            // Container for all data to post
            // We must do this in batches later
            var data = new Data();

            // Categories (existing categories may be redundant, will check how save data behaves) 
            var categories = new List<Models.Litium.Category>();

            categories = CategoryService.ProcessCategoryCVLsProductCategories(headCategory, 1, categories);
            categories = CategoryService.ProcessCategoryCVLsProductCategories(productCategories, 2, categories, headCategory);
            categories = CategoryService.ProcessCategoryCVLsProductCategories(productGroups, 3, categories, productCategories);
           

            (string assortmentId, List<string> existingCategorys) = CategoryService.GetAssortmentIdAndExistingCategoryIds();

            // Resources ??

            // Products
            ProductService.ProcessProducts(products, data, cultures, assortmentId, existingCategorys, categories);

            // Variants
            VariantService.ProcessVariants(items, data, cultures);

            // Save data to Litium
            LitiumCommonService.SaveData(data);

           
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Constants
{
    public class InRiver
    {
        public class Remoting
        {
            public const string InRiverRemotingUrl = "https://remoting.productmarketingcloud.com";
            public const string InRiverUsername = "inriver-tengtools@consid.se";
            public const string InRiverPassword = "544%%IdkwDWHk\"XgbeU3pdD"; //notera escapat " i lösen... Autogenerat :) 544%%IdkwDWHk"XgbeU3pdD
            public const string InRiverEnvironmentProd = "prod";
            public const string InRiverEnvironmentTest = "test";
        }

        public class CVL
        {
            public const string Category = "Category";
            public const string ProductGroup = "ProductGroup";
            public const string HeadCategory = "HeadCategory";
        }

        public class EntityType
        {
            public const string ProductTypeId = "Product";
            public const string ItemTypeId = "Item";
            public const string ResourceTypeId = "Resource";
            public const string CategoryTypeId = "Category";
        }

        public class LinkType
        {
            public const string ProductItem = "ProductItem";
        }

        public class InRiverField
        {
            public class Product
            {
                public const string ProductId = "ProductID";
                public const string ProductShortDescription = "ProductShortDescription";
            }
            public class Item
            {
                public const string ItemId = "ItemID";
                public const string ItemShortDescription = "ItemShortDescription";
            }
            public class Category
            {
                
            }
        }
    }
}

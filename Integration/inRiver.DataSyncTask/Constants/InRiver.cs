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
            public const string InRiverPassword = "544%%IdkwDWHk\"XgbeU3pdD"; //notera escapat " i lösen... Autogenerat :)
            public const string InRiverEnvironmentProd = "prod";
            public const string InRiverEnvironmentTest = "test";
        }

        public class CVL
        {
            public const string HeadCategory = "HeadCategory";
            public const string Category = "Category";
            public const string ProductGroup = "ProductGroup";
        }

        public class EntityType
        {
            public const string ChannelTypeId = "Channel";
            public const string ProductTypeId = "Product";
            public const string ItemTypeId = "Item";
            public const string ResourceTypeId = "Resource";
            public const string CategoryTypeId = "Category";
        }

        public class LinkType
        {
            public const string ProductItem = "ProductItem";
        }


        // None dynamic field, not mapped over from inRiver to Litium as datafields per se. Used for validation etc
        public class InRiverField
        {
            public class Product
            {
                public const string ProductId = "ProductID";
                
                //CVLS BELOW
                public const string ProductPublicPlatforms = "ProductPublicPlatforms";
                public const string ProductMarket = "ProductMarket";

                // INT
                public const string ProductPriority = "ProductPriority";
            }

            public class Item
            {
                public const string ItemId = "ItemID";
                public const string ItemApprovedForMarket = "ItemApprovedForMarket";
            }

            public class Category
            {
            }
        }
    }
}

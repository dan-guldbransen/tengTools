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
            public const string Category = "Category";
            public const string ProductGroup = "ProductGroup";
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
                
                //CVLS BELOW
                public const string ProductBrand = "ProductBrand";
                public const string ProductPublicPlatforms = "ProductPublicPlatforms";

                public const string ProductCategoryNumber = "ProductCategoryNumber";
                public const string ProductCategoryDescription = "ProductCategoryDescription";
                public const string ProductGroupNumber = "ProductGroupNumber";
                public const string ProductGroupDescription = "ProductGroupDescription";
                public const string ProductMarket = "ProductMarket";

                // LOCALESTRING BELOW
                public const string ProductShortDescription = "ProductShortDescription";
                public const string ProductLongDescription = "ProductLongDescription";
                public const string ProductShortHeading = "ProductShortHeading";
                public const string ProductLongHeading = "ProductLongHeading";
                
                public const string ProductBullet1 = "ProductBullet1";
                public const string ProductBullet2 = "ProductBullet2";
                public const string ProductBullet3 = "ProductBullet3";
                public const string ProductBullet4 = "ProductBullet4";
                public const string ProductBullet5 = "ProductBullet5";

                public const string ProductMaterial = "ProductMaterial";
                public const string ProductKeywords = "ProductKeywords"; //; separated list
                public const string ProductMetaTitle = "ProductMetaTitle";
                public const string ProductMetaDescription = "ProductMetaDescription";

                // INT
                public const string ProductPriority = "ProductPriority";
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

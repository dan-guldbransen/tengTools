using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.inRiver
{
    public class CategoryStructure
    {
        public List<inRiverCat> HeadCategories { get; set; } = new List<inRiverCat>();
    } 

    public class inRiverCat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string ProductCategoryNumber { get; set; }
        public string ProductGroupNumber { get; set; }
        
        public List<inRiverCat> Categories { get; set; } = new List<inRiverCat>();
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}

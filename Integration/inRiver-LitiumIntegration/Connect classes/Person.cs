using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Person
    {
        public string id { get; set; }
        public string fieldTemplateId { get; set; }
        public List<string> organizationIds { get; set; }
        public List<Address> addresses { get; set; }
        public List<Field> fields { get; set; }
        public List<string> priceGroupIds { get; set; }
    }
}

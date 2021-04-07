using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class Field
    {
        public Field(inRiver.Remoting.Objects.Field f)
        {
            fieldDefinitionId = f.FieldType.Id;
            value = f.Data;
        }

        public string fieldDefinitionId { get; set; }
        public string culture { get; set; }
        public object value { get; set; }
    }
}

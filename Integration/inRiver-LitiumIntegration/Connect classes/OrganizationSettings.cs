using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver_LitiumIntegration.Connect_classes
{
    public class OrganizationSettings
    {
        public string fieldTemplateId { get; set; }
        public string vatRegistrationNumberFieldDefinitionId { get; set; }
        public string legalRegistrationNumberFieldDefinitionId { get; set; }
        public AddressTypeMapping addressTypeMapping { get; set; }
    }
}

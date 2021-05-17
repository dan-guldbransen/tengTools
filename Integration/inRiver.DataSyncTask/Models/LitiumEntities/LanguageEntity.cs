using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Models.LitiumEntities
{
    public class LanguageEntity
    {
        public List<Dictionary<string, string>> FallbackLanguages { get; set; }

        public string Id { get; set; }
        public bool IsDefaultLanguage { get; set; }
        public string SystemId { get; set; }
    }
}

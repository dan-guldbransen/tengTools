using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Utils
{
    public class JsonFileReader
    {
       public static List<PimLitiumFieldMap> Read(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                try
                {
                    string json = file.ReadToEnd();

                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    return JsonConvert.DeserializeObject<List<PimLitiumFieldMap>>(json, serializerSettings);
                }
                catch (Exception)
                {
                    Console.WriteLine("Problem reading file");
                    return null;
                }
            }
        }
    }
}

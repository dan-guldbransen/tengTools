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
        public static FieldMapper Read()
        {
            var path = Directory.GetCurrentDirectory();

            using (StreamReader file = new StreamReader(path + "/fieldconfig.json"))
            {
                try
                {

                    string json = file.ReadToEnd();

                    var serializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    return JsonConvert.DeserializeObject<FieldMapper>(json, serializerSettings);
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

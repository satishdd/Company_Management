using Newtonsoft.Json;

namespace Company_Management.FileWriter
{
    public class JsonFileUtils
    {
        private static readonly JsonSerializerSettings _options = new() { NullValueHandling = NullValueHandling.Ignore };

        public static void SimpleWrite(object obj, string fileName)
        {
            var jsonString = JsonConvert.SerializeObject(obj, _options);
            File.WriteAllText(fileName, jsonString);
        }
    }
}


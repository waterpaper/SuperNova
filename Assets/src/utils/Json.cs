using Newtonsoft.Json;

namespace Supernova.Utils
{
    public class Json
    {
        public static string ObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
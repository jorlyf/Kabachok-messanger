using Newtonsoft.Json;

namespace Serializer
{
    public static class SerializeService
    {
        public static string Serialize(SendData data)
        {
            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public static SendData Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<SendData>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}

using Newtonsoft.Json;

namespace Serializer
{
	public static class SerializeService
	{
		public static string Serialize(object data)
		{
			return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
		}

		public static SendData Deserialize(string data)
		{
			return JsonConvert.DeserializeObject<SendData>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto});
		}
	}
}

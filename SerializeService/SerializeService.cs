using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SerializeService
{
    public static class SerializeService
    {
        public static byte[] Serialize(SendData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        public static void Deserialize(byte[] buffer)
        {

        }
    }
}

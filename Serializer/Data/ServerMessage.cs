using System;

namespace Serializer.Data
{
    public class ServerMessage : Message
    {
        public int Key { get; set; }
        public ServerMessage(string text, string login, int key) : base(text, login)
        {
            Key = key;
        }
    }
}

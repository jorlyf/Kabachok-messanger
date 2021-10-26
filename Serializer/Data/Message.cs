using System;

namespace Serializer.Data
{
    public class Message
    {
        public string SenderLogin;
        public string Text;
        public DateTime Date = DateTime.Now;

        public Message(string text, string senderLogin)
        {
            Text = text;
            SenderLogin = senderLogin;
        }

    }
}

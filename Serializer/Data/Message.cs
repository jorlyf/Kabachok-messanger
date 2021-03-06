using System;

namespace Serializer.Data
{
    public class Message
    {
        public string SenderLogin { get; set; }
        public string Text { get; set; }
        public string Time { get; set; } = DateTime.Now.ToLocalTime().ToShortTimeString();

        public Message(string text, string senderLogin)
        {
            Text = text;
            SenderLogin = senderLogin;
        }

    }
}

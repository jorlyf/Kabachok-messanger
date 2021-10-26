using System;

namespace Serializer
{
    public class SendData // container for Data
    {
        public dynamic Data;
        public Type Type; // type of object Data

        public string SenderLogin;
        public DateTime Date = DateTime.Now;

        public SendData(object data, string senderLogin)
        {
            Data = data;
            Type = Data.GetType();
            SenderLogin = senderLogin;
        }
    }
}

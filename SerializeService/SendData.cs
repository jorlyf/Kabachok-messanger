using System;

namespace SerializeService
{
    [Serializable]
    public class SendData
    {
        public object Object;
        //public Type Type;

        public string SenderLogin;
        public DateTime Date = DateTime.Now;

        public SendData(object obj, string senderLogin)
        {
            Object = obj;
            //Type = Object.GetType();
            SenderLogin = senderLogin;
        }
    }
}

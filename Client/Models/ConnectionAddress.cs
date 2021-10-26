using System.Net;

namespace Client.Models
{
    public class ConnectionAddress
    {
        public string IP { get; set; } = "";

        private int _Port = 0;
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }
    }
}

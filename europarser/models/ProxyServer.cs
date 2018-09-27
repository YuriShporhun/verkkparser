using europarser.interfaces;
using System.Net;

namespace europarser.models
{
    public class ProxyServer
    {
        public IPAddress IP { get; set; }
        public IIPOwner Owner { get; set; }
    }
}

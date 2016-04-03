using System.Net.Sockets;

namespace CONetwork
{
    public class Wrapper
    {
        public ushort Port;
        public Socket Socket;
        public byte[] Buffer;
        public object Connector;
    }
}

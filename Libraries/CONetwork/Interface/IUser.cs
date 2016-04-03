using System.Net.Sockets;

namespace CONetwork.Interface
{
    public interface IUser
    {
        Socket Socket { get; set; }
        uint Uid { get; }
        void Send(byte[] data);
    }
}
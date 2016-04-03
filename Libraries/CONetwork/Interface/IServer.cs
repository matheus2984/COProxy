namespace CONetwork.Interface
{
    public interface IServer : IProtocol
    {
        event SocketEvent<IUser, object> OnConnect;
        event SocketEvent<IUser, object> OnDisconnect;
        event SocketEvent<IUser, byte[]> OnReceive;
    }
}
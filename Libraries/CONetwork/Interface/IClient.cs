namespace CONetwork.Interface
{
    public interface IClient:IProtocol
    {
        event SocketEvent<IClient, object> OnConnect;
        event SocketEvent<IClient, object> OnConnectionFailed;
        event SocketEvent<IClient, object> OnDisconnect;
        event SocketEvent<IClient, byte[]> OnReceive;
    }
}
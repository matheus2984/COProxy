namespace CONetwork.Interface
{
    public delegate void SocketEvent<in T, in T2>(T sender, T2 arg);
    
    public interface IProtocol
    {
        string IP { get; }
        int Port { get; }
    }
}
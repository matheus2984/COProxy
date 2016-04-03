using COAPI;

namespace COGameProxy.Network
{
    public class PacketHandler : COAPI.PacketHandler
    {
        private readonly GameUser role;
        private readonly ClientHandler clientHandler;
        private readonly ServerHandler serverHandler;

        public PacketHandler(GameUser role) : base(role)
        {
            this.role = role;
            clientHandler = new ClientHandler(role);
            serverHandler = new ServerHandler(role);
        }

        public override void SplitClient(byte[] data)
        {
            role.ClientQueue.Push(data);
            while (role.ClientQueue.CanPop())
            {
                clientHandler.Handle(role.ClientQueue.Pop());
            }
        }

        public override void SplitServer(byte[] data)
        {
            role.ServerQueue.Push(data);
            while (role.ServerQueue.CanPop())
            {
                serverHandler.Handle(role.ServerQueue.Pop());
            }
        }
    }
}
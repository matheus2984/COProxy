namespace COAPI
{
    public abstract class PacketHandler
    {
        private readonly GameUser role;

        protected PacketHandler(GameUser who)
        {
            role = who;
        }

        public abstract void SplitClient(byte[] data);
        public abstract void SplitServer(byte[] data);
    }
}
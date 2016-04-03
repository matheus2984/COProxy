namespace COPacketLibrary
{
    public interface IPacket<out T>
    {
        byte[] Serialize();
        T Deserialize();
    }
}
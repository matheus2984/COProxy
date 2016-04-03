using System.Drawing;

namespace COPacketLibrary
{
    public class Msg : Packet<Msg>
    {
        public string Message;
        public string From;
        public string To;
        public uint ChatType;
        public Color Color;
        public uint Mesh;
        public uint MessageUID1;
        public uint MessageUID2;

        public Msg(string seal) : base((ushort) PacketType.HeroInformation, seal)
        {

        }

        public Msg(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            Seek(8);

            return GetData();
        }

        public override Msg Deserialize()
        {
            Seek(8);
            // Color = Color.FromArgb(ReadInt());
            //    ChatType = ReadUInt();
            //  MessageUID1 = ReadUInt();
            //   MessageUID2 = ReadUInt();
            //    Mesh = ReadUInt();
            Seek(29);
            var fromSize = ReadByte();
            From = ReadString(fromSize);
            var toSize = ReadByte();
            To = ReadString(toSize);
            var unknow = ReadByte();
            var messageSize = ReadByte();
            Message = ReadString(messageSize);

            return this;
        }
    }
}
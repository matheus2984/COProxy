namespace COPacketLibrary
{
    public class HeroInformation : Packet<HeroInformation>
    {
        public uint UID { get; set; }

        private byte NameLength { get; set; }

        public string Name { get; set; }

        public HeroInformation(string seal) : base((ushort) PacketType.HeroInformation, seal)
        {

        }

        public HeroInformation(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            Seek(8);
            Write(UID);
            Seek(123);
            Write((byte)Name.Length);
            Write(Name);
            return GetData();
        }


        public override HeroInformation Deserialize()
        {
            Seek(8);
            UID = ReadUInt();
            Seek(123);
            NameLength = ReadByte();
            Name = ReadString(NameLength);
            return this;
        }
    }
}
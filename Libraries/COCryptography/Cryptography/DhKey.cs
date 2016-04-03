using System.IO;
using System.Text;

namespace COCryptography.Cryptography
{
    public class ClientDHPacket
    {
        public readonly string ClientPubKey;
        private readonly int junkLength;

        public ClientDHPacket(byte[] packet)
        {
            MemoryStream MS = new MemoryStream(packet);
            BinaryReader BR = new BinaryReader(MS);
            BR.ReadBytes(7);//JUNK
            BR.ReadUInt32();//Length
            junkLength = BR.ReadInt32();
            BR.ReadBytes(junkLength);
            ClientPubKey = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
            BR.Close();
            MS.Close();
        }
        public void Edit(byte[] Packet, string NewKey)
        {
            MemoryStream MS = new MemoryStream(Packet);
            BinaryWriter BW = new BinaryWriter(MS);
            BW.Seek(19 + junkLength, SeekOrigin.Current);
            BW.Write(Encoding.ASCII.GetBytes(NewKey));
        }
    }
    public class ServerDHPacket
    {
        public byte[] ServerIV;
        public byte[] ClientIV;
        public string P;
        public string G;
        public string Server_PubKey;
        private readonly int JunkLength;

        public ServerDHPacket(byte[] packet)
        {
            MemoryStream MS = new MemoryStream(packet);
            BinaryReader BR = new BinaryReader(MS);
            BR.ReadBytes(11);//JUNK
            BR.ReadUInt32();//Length - Like i care of it
            JunkLength = BR.ReadInt32();
            BR.ReadBytes(JunkLength);//JUNK length
            ServerIV = BR.ReadBytes(BR.ReadInt32());
            ClientIV = BR.ReadBytes(BR.ReadInt32());
            P = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
            G = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
            Server_PubKey = Encoding.ASCII.GetString(BR.ReadBytes(BR.ReadInt32()));
            BR.Close();
            MS.Close();
        }

        public void Edit(byte[] packet, string editedPubKey)
        {
            MemoryStream ms = new MemoryStream(packet);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Seek(55 + JunkLength + P.Length + G.Length, SeekOrigin.Current);
            bw.Write(Encoding.ASCII.GetBytes(editedPubKey));
            bw.Close();
            ms.Close();
        }
    }
}

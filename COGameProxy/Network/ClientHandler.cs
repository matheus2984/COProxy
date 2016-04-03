using System;
using COAPI;
using COCryptography;
using COPacketLibrary;

namespace COGameProxy.Network
{
    public class ClientHandler
    {
        private GameUser role;
        public ClientHandler(GameUser role)
        {
            this.role = role;
        }

        public void Handle(byte[] data)
        {
            var send = true;
            try
            {
                Logger.Instance.AddData(data);
                var type = (PacketType) BitConverter.ToUInt16(data, 2);
                switch (type)
                {
                    case PacketType.Msg:
                        send = Process_Msg(new Msg(data).Deserialize(), ref data);
                        break;
                    default:
                        //   Console.WriteLine("Unknown packet type: " + (ushort)type);
                        break;
                }
                if (send)
                    role.SendToServer(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private bool Process_Msg(Msg packet, ref byte[] data)
        {
            Console.WriteLine(packet.Message);
            if (packet.Message.StartsWith("@") || packet.Message.StartsWith("/"))
            {
                string[] command = packet.Message.Substring(1).ToLower().Split(' ');
                switch (command[0])
                {
                    case "speed":
                        Speedhack();
                        break;
                }
                return false;
            }
            return true;
        }

        public void Speedhack()
        {
            byte[] packet = new byte[84 + 8 + 4];
            ReadWrite.WriteUInt16(84 + 4, 0, packet);
            ReadWrite.WriteUInt16(10017, 2, packet);
            ReadWrite.WriteUInt32(role.UID, 4 + 4, packet);
            ReadWrite.WriteUInt32(1, 8 + 4, packet);
            ReadWrite.WriteUInt32(25, 12 + 4, packet);
            ReadWrite.WriteUInt64(role.statusflag | 0x800000, 16 + 4, packet);
            ReadWrite.WriteString("TQServer", packet.Length - 8, packet);
            role.SendToClient(packet);
        }
    }
}
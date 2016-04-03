using System;
using COAPI;
using COPacketLibrary;

namespace COGameProxy.Network
{
   public class ServerHandler
   {
       private readonly GameUser role;
       public ServerHandler(GameUser role)
       {
           this.role = role;
       }

        public void Handle(byte[] data)
       {
            var send = true;
            try
            {
                Logger.Instance.AddData(data);
                var type = (PacketType)BitConverter.ToUInt16(data, 2);
                switch (type)
                {
                    case PacketType.HeroInformation:
                        send = Process_HeroInformation(new HeroInformation(data).Deserialize(), ref data);
                        break;
                    default:
                        //  Console.WriteLine("Unknown packet type: " + (ushort)type);
                        break;
                }
                if (send)
                    role.SendToClient(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private bool Process_HeroInformation(HeroInformation packet, ref byte[] data)
        {
            Console.WriteLine(packet.Name);
            role.UID = packet.UID;
            role.Name = packet.Name;
            role.namelen = (byte)packet.Name.Length;

            return true;
        }
    }
}

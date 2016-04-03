using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using COCryptography;
using COCryptography.Cryptography;
using CONetwork;
using CONetwork.Util;
using OpenSSL;
using Random = System.Random;

namespace COAPI
{
    public class GameUser
    {
        public static Random RND = new Random();
        private readonly Socket clientConnection;
        public readonly PacketQueue ClientQueue = new PacketQueue();

        private readonly ClientSocket serverConnection;

        public readonly PacketQueue ServerQueue = new PacketQueue();

        /* public ConcurrentDictionary<uint, Objects.Monster> SurroundingMobs =
            new ConcurrentDictionary<uint, Objects.Monster>();*/

        public ushort AimbotSkillToReplace, AimbotSkillToUse;
        public GameCrypto ClientCrypt;
        public ClientDHPacket ClientDataDHP;
        public bool exchanging = true;
        public ulong Experience, statusflag;
        public ushort HitPoints, Mana, PKP, prevX, prevY;
        public bool LastJumpConfirmed = true, hunting = false, OnXp = false, Aimbot = false, finishedPF = true;
        public byte level, proff, firstreb, secondreb, reborn, namelen, spouselen;
        public DateTime LoggedIn = DateTime.Now.AddSeconds(60);
        public uint money, conquerpoints, boundedconquerpoints;
        public string Name = "Char" + RND.Next(10000), spousname;

        public PacketHandler PacketHandler;

        public List<Point> Pathfinding = new List<Point>();

        public bool PFStuckedX = false,
            PFStuckedY = false,
            finishedX = false,
            finishedY = false,
            ValidXescapeR = true,
            ValidYescapeR = true,
            ValidXescapeL = true,
            ValidYescapeL = true;

        public GameCrypto ServerCrypt;
        public ServerDHPacket ServerDataDHP;
        public uint UID, updaterate = 1, updateratecounter = 0;
        public uint xp;

        public ushort MapID { get; set; }

        public ushort X { get; set; }

        public ushort Y { get; set; }

        public GameUser(Wrapper inSock)
        {
            clientConnection = inSock.Socket;

            ClientCrypt = new GameCrypto(Encoding.ASCII.GetBytes(Constants.EncryptionKey));
            ClientCrypt.Blowfish.EncryptIV = new byte[8];
            ClientCrypt.Blowfish.DecryptIV = new byte[8];

            ServerCrypt = new GameCrypto(Encoding.ASCII.GetBytes(Constants.EncryptionKey));
            ServerCrypt.Blowfish.EncryptIV = new byte[8];
            ServerCrypt.Blowfish.DecryptIV = new byte[8];

            serverConnection = new ClientSocket(Constants.GameIP, Constants.GamePort);
            serverConnection.Connect();
            serverConnection.OnConnect += ServerConnection_OnConnect;
            serverConnection.OnDisconnect += ServerConnection_OnDisconnect;
            serverConnection.OnReceive += ServerConnection_OnReceive;

            if (serverConnection.Connected)
                Console.WriteLine("[GameServer] Connected to TQ Game server");
        }

        private void ServerConnection_OnConnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            Console.WriteLine("[GameServer] Connected to game server");
        }

        private void ServerConnection_OnReceive(object sender, CONetwork.Events.SocketReceiveEventArgs e)
        {
            ClientCrypt.Decrypt(e.Data);
            if (exchanging)
            {
                ServerDataDHP = new ServerDHPacket(e.Data);
                ClientCrypt.Dh = new DH(BigNumber.FromHexString(ServerDataDHP.P),
                    BigNumber.FromHexString(ServerDataDHP.G));
                ServerCrypt.Dh = new DH(BigNumber.FromHexString(ServerDataDHP.P),
                    BigNumber.FromHexString(ServerDataDHP.G));
                ClientCrypt.Dh.GenerateKeys();
                ServerCrypt.Dh.GenerateKeys();
                Console.WriteLine("P [" + ServerDataDHP.P + "]");
                Console.WriteLine("G [" + ServerDataDHP.G + "]");
                Console.WriteLine("ServerPublicKey [" + ServerDataDHP.Server_PubKey + "]");
                ServerDataDHP.Edit(e.Data, ServerCrypt.Dh.PublicKey.ToHexString());
                SendToClient(e.Data);
            }
            else
                PacketHandler.SplitServer(e.Data);
        }

        private void ServerConnection_OnDisconnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            Console.WriteLine("[GameServer] Disconnected from game server");
            Disconnect();
        }

        public bool SendToServer(byte[] data)
        {
            try
            {
                if (serverConnection.Connected)
                {
                    if (data.Length > 8 && !exchanging)
                        ReadWrite.WriteString("TQClient", data.Length - 8, data);
                    lock (ClientCrypt)
                        ClientCrypt.Encrypt(data);
                    serverConnection.Send(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool SendToClient(byte[] data)
        {
            try
            {
                if (clientConnection.Connected)
                {
                    if (data.Length > 8 && !exchanging)
                        ReadWrite.WriteString("TQServer", data.Length - 8, data);
                    lock (ServerCrypt)
                        ServerCrypt.Encrypt(data);
                    clientConnection.Send(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (serverConnection.Connected)
                    serverConnection.Disconnect();
                if (clientConnection.Connected)
                    clientConnection.Disconnect(false);
            }
            catch
            {
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using COAPI;
using COCryptography.Cryptography;
using CONetwork;
using OpenSSL;
using PacketHandler = COGameProxy.Network.PacketHandler;

namespace COGameProxy
{
    public class GameProxy
    {
        public GameProxy()
        {
            var game = new ServerSocket(Constants.GamePort, Constants.GamePort);
            game.OnReceive += game_OnReceive;
            game.OnConnect += game_OnConnect;
            game.OnDisconnect += game_OnDisconnect;

            Logger.Instance.Show();
            Logger.Instance.Text = "COGameProxy";
        }

        private void game_OnConnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            var role = new GameUser(e.Wrapper);
            role.PacketHandler = new PacketHandler(role);
            e.Wrapper.Connector = role;
        }

        private void game_OnReceive(object sender, CONetwork.Events.SocketReceiveEventArgs e)
        {
            if (!(e.Wrapper.Connector is GameUser))
            {
                e.Wrapper.Socket.Disconnect(false);
                return;
            }
            var role = (GameUser)e.Wrapper.Connector;
            lock (role.ServerCrypt)
                role.ServerCrypt.Decrypt(e.Data);
            if (role.exchanging)
            {
                role.ClientDataDHP = new ClientDHPacket(e.Data);
                Console.WriteLine("ClientPublicKey [" + role.ClientDataDHP.ClientPubKey + "]");
                role.ClientDataDHP.Edit(e.Data, role.ClientCrypt.Dh.PublicKey.ToHexString());
                role.SendToServer(e.Data);
                SetUpCrypto(role);
            }
            else
                role.PacketHandler.SplitClient(e.Data);
        }

        private void game_OnDisconnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            var user = e.Wrapper.Connector as GameUser;
            if (user != null)
            {
                GameUser role = user;
                role.Disconnect();
                role.hunting = false;
            }
            else
                e.Wrapper.Socket.Disconnect(false);
            Console.WriteLine("[GameServer] Client>Server Disconnection");
        }

        #region Cryptos

        private void SetUpCrypto(GameUser role)
        {
            try
            {
                BigNumber realClientPublicKey = BigNumber.FromHexString(role.ClientDataDHP.ClientPubKey);
                BigNumber realServerPublicKey = BigNumber.FromHexString(role.ServerDataDHP.Server_PubKey);

                var clientCrypto = new GameCrypto(DHKey(role.ClientCrypt.Dh.ComputeKey(realServerPublicKey)));
                Console.WriteLine("FAClientPublicKey [" + role.ClientCrypt.Dh.PublicKey.ToHexString() + "]");

                var serverCrypto = new GameCrypto(DHKey(role.ServerCrypt.Dh.ComputeKey(realClientPublicKey)));
                Console.WriteLine("FAServerPublicKey [" + role.ServerCrypt.Dh.PublicKey.ToHexString() + "]");

                clientCrypto.Blowfish.EncryptIV = role.ServerDataDHP.ClientIV;
                clientCrypto.Blowfish.DecryptIV = role.ServerDataDHP.ServerIV;
                Console.WriteLine("Done IVs1 .");

                serverCrypto.Blowfish.EncryptIV = role.ServerDataDHP.ServerIV;
                serverCrypto.Blowfish.DecryptIV = role.ServerDataDHP.ClientIV;
                Console.WriteLine("Done IVs2 .");

                role.ClientCrypt = clientCrypto;
                role.ServerCrypt = serverCrypto;
                Console.WriteLine("Done Cryptos .");

                role.exchanging = false;
                role.LoggedIn = DateTime.Now.AddSeconds(2);
            }
            catch
            {
            }
        }

        public byte[] DHKey(byte[] key)
        {
            var MD5 = new MD5CryptoServiceProvider();
            string text = HexString(MD5.ComputeHash(key, 0, key.TakeWhile(x => x != 0).Count()));
            string str = HexString(MD5.ComputeHash(Encoding.ASCII.GetBytes(text + text)));
            string s = text + str;
            return Encoding.ASCII.GetBytes(s);
        }

        private string HexString(byte[] bytes)
        {
            var array = new char[bytes.Length*2];
            var i = 0;
            var num = 0;
            while (i < bytes.Length)
            {
                var num2 = (byte) (bytes[i] >> 4);
                array[num] = (char) (num2 > 9 ? num2 + 55 + 32 : num2 + 48);
                num2 = (byte) (bytes[i] & 15);
                array[++num] = (char) (num2 > 9 ? num2 + 55 + 32 : num2 + 48);
                i++;
                num++;
            }
            return new string(array);
        }

        #endregion
    }
}
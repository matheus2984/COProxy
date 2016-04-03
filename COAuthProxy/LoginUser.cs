using System;
using System.Net.Sockets;
using System.Text;
using COAPI;
using COCryptography;
using COCryptography.Cryptography;
using CONetwork;
using CONetwork.Events;

namespace COAuthProxy
{
    public class LoginUser
    {
        public readonly AuthProtocolCryptographer ClientAuthCryptography;
        private readonly Socket clientConnection;
        private readonly AuthProtocolCryptographer serverAuthCryptography;
        private readonly ClientSocket serverConnection;

        public LoginUser(Wrapper inSock)
        {
            ClientAuthCryptography = new AuthProtocolCryptographer();
            serverAuthCryptography = new AuthProtocolCryptographer();

            clientConnection = inSock.Socket;
            serverConnection = new ClientSocket(Constants.AUTH_IP, inSock.Port);
            serverConnection.Connect();
            serverConnection.OnConnect += serverConnection_OnConnect;
            serverConnection.OnReceive += serverConnection_OnReceive;
            serverConnection.OnConnectionFailed += serverConnection_OnConnectionFailed;

            serverConnection.OnDisconnect += serverConnection_OnDisconnect;
            if (serverConnection.Connected) Console.WriteLine("[LoginServer] Connected to Auth Server successfully!");
        }
        
        private void serverConnection_OnConnect(object sender, SocketConnectionEventArgs e)
        {
            Console.WriteLine("[LoginServer] Connected to Auth Server successfully!");
        }

        private void serverConnection_OnConnectionFailed(object sender, SocketConnectionEventArgs e)
        {
            Console.WriteLine("[LoginServer] Failed to Connected to Auth Server successfully!");
        }

        private void serverConnection_OnReceive(object sender, SocketReceiveEventArgs e)
        {
            lock (serverAuthCryptography)
                serverAuthCryptography.DecryptBackwards(e.Data);

            // Logger.Instance.AddData(data);
            ushort type = BitConverter.ToUInt16(e.Data, 2);
            switch (type)
            {
                case 1055: //auth response
                    if (e.Data.Length > 39) //approved login.
                    {
                        Constants.GameIP = ReadWrite.ReadString(e.Data, 20, 16).Replace("\0", "");
                        for (var i = 20; i < 36; i++)
                            e.Data[i] = 0;
                        Buffer.BlockCopy(Encoding.ASCII.GetBytes(Constants.ProxyIP), 0, e.Data, 20,
                            Math.Min(Constants.ProxyIP.Length, 16));
                    }
                    SendToClient(e.Data);
                    break;
                default:
                    SendToClient(e.Data);
                    break;
            }
        }

        private void serverConnection_OnDisconnect(object sender, SocketConnectionEventArgs e)
        {
            Disconnect();
        }

        #region Disconnect

        private void Disconnect()
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
                // ignored
            }
        }

        #endregion

        #region SendMethods

        public bool SendToServer(byte[] data)
        {
            try
            {
                if (serverConnection.Connected)
                {
                    lock (ClientAuthCryptography)
                    {
                        ClientAuthCryptography.EncryptBackwards(data);
                        serverConnection.Send(data);
                    }
                }
                else
                {
                    Console.WriteLine("Error! Not connected to tq auth server");
                    Disconnect();
                }
                return true;
            }
            catch (Exception p)
            {
                Console.WriteLine(p);
                return false;
            }
        }

        public bool SendToClient(byte[] data)
        {
            try
            {
                if (clientConnection.Connected)
                {
                    lock (serverAuthCryptography)
                    {
                        serverAuthCryptography.Encrypt(data);
                        clientConnection.Send(data);
                    }
                }
                else
                {
                    Console.WriteLine("Error! Not connected to tq auth server");
                    Disconnect();
                }
                return true;
            }

            catch (Exception p)
            {
                Console.WriteLine(p);
                return false;
            }
        }

        #endregion
    }
}
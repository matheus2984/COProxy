using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using CONetwork.Events;
using CONetwork.Interface;
using CONetwork.Util;

namespace CONetwork
{
    public class ClientSocket 
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        
        private readonly Socket socket;

        public event EventHandler<SocketConnectionEventArgs> OnConnect;
        public event EventHandler<SocketConnectionEventArgs> OnConnectionFailed;
        public event EventHandler<SocketReceiveEventArgs> OnReceive; 
        public event EventHandler<SocketConnectionEventArgs> OnDisconnect; 

        public bool Connected { get { return socket.Connected; } }

        public ClientSocket(string ip, int port)
        {
            IP = ip;
            Port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {NoDelay = false};
        }

        public void Connect()
        {
            if (socket.Connected) throw new Exception("O socket ja esta conectado");
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IP), Port), ConnectCallback, new Wrapper());
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (socket.Connected)
            {
                var wr = result.AsyncState as Wrapper;
              //  socket.EndConnect(result);
                wr.Socket = socket;
                wr.Port = (ushort)Port;
                wr.Buffer = new byte[2048];

                OnConnect.SafeInvoke(this, null);

                socket.BeginReceive(wr.Buffer, 0, wr.Buffer.Length, SocketFlags.None, ReceivedCallback, wr);
            }
            else
            {
                OnConnectionFailed.SafeInvoke(this, null);
            }
        }   

        private void ReceivedCallback(IAsyncResult result)
        {
            try
            {
                var wr = result.AsyncState as Wrapper;
                int bufLength = socket.EndReceive(result);
                var packet = new byte[bufLength];
                Array.Copy(wr.Buffer, packet, packet.Length);

                OnReceive.SafeInvoke(this, new SocketReceiveEventArgs(wr, packet));

                wr.Buffer = new byte[2048];
                socket.BeginReceive(wr.Buffer, 0, wr.Buffer.Length, SocketFlags.None, ReceivedCallback, wr);
            }
            catch
            {
                Disconnect();
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, Send_Callback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private void Send_Callback(IAsyncResult result)
        {
            try
            {
                socket.EndSend(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("sendcallback:  "+ex);
                Disconnect();
            }
        }

        private bool disconnectInvoked;
        public void Disconnect()
        {
            if (disconnectInvoked) return;
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            OnDisconnect.SafeInvoke(this, null);
            disconnectInvoked = true;
        }
    }
}
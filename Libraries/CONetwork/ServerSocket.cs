using System;
using System.Net;
using System.Net.Sockets;
using CONetwork.Events;
using CONetwork.Util;

namespace CONetwork
{
    public class ServerSocket 
    {

        private readonly Socket socket;
        public readonly ushort OutPort;

        public event EventHandler<SocketReceiveEventArgs> OnReceive;
        public event EventHandler<SocketConnectionEventArgs> OnConnect;
        public event EventHandler<SocketConnectionEventArgs> OnDisconnect;

        public ServerSocket(ushort internalPort, ushort externalPort)
        {
            OutPort = externalPort;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, internalPort));
            socket.Listen(500);
           // Socket.NoDelay = false;
            socket.BeginAccept(Accept, new Wrapper());
        }

        private void Accept(IAsyncResult result)
        {
            try
            {
                var wr = result.AsyncState as Wrapper;
                wr.Socket = socket.EndAccept(result);
                wr.Port = OutPort;
                wr.Buffer = new byte[2048];
                wr.Socket.BeginReceive(wr.Buffer, 0, 2048, SocketFlags.None, Receive, wr);
                OnConnect.SafeInvoke(this, new SocketConnectionEventArgs(wr));

                socket.BeginAccept(Accept, new Wrapper());
            }
            catch
            {
                // ignored
            }
        }

        private void Receive(IAsyncResult result)
        {
            try
            {
                var wr = result.AsyncState as Wrapper;
                if (!wr.Socket.Connected)
                    return;
                var error = SocketError.Disconnecting;
                int size = wr.Socket.EndReceive(result, out error);
                if (error == SocketError.Success && size != 0)
                {
                    var buffer = new byte[size];
                    Buffer.BlockCopy(wr.Buffer, 0, buffer, 0, size);
                    OnReceive.SafeInvoke(this, new SocketReceiveEventArgs(wr, buffer));
                    if (wr.Socket.Connected)
                        wr.Socket.BeginReceive(wr.Buffer, 0, 2048, SocketFlags.None, Receive, wr);
                }
                else
                {
                    if (wr.Socket.Connected)
                    {
                        wr.Socket.Disconnect(true);
                    }
                    try
                    {
                        OnDisconnect.SafeInvoke(this, new SocketConnectionEventArgs(wr));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
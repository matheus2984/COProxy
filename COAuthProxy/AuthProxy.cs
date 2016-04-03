using System;
using COAPI;
using CONetwork;

namespace COAuthProxy
{
    public class AuthProxy
    {
        public AuthProxy()
        {
            var auth = new ServerSocket(Constants.AUTH_PORT, Constants.AUTH_PORT);
            auth.OnConnect += auth_OnConnect;
            auth.OnDisconnect += auth_OnDisconnect;
            auth.OnReceive += auth_OnReceive;

          //  Logger.Instance.Show();
           // Logger.Instance.Text = "COAuthProxy";   
        }

        private void auth_OnConnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            Console.WriteLine("[LoginServer] Client connected to Proxy");
            var user = new LoginUser(e.Wrapper);
            e.Wrapper.Connector = user;
        }

        private void auth_OnReceive(object sender, CONetwork.Events.SocketReceiveEventArgs e)
        {
            try
            {
                var role = e.Wrapper.Connector as LoginUser;
                if (role == null) return;
                lock (role.ClientAuthCryptography)
                    role.ClientAuthCryptography.Decrypt(e.Data);

                //   Logger.Instance.AddData(data);
                role.SendToServer(e.Data);
            }
            catch (Exception p)
            {
                Console.WriteLine(p);
            }
        }

        private void auth_OnDisconnect(object sender, CONetwork.Events.SocketConnectionEventArgs e)
        {
            Console.WriteLine("[LoginServer] Client disconnected");
        }
    }
}
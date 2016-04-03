using System;
using System.Windows.Forms;
using COAuthProxy;
using COGameProxy;

namespace COProxy
{
    internal static class Program
    {
        private static AuthProxy authProxy;
        private static GameProxy gameProxy;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            authProxy = new AuthProxy();
            gameProxy = new GameProxy();
            Application.Run();
            //Application.Run(Logger.Instance);


            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
using Logger;

namespace COGameProxy
{
    public static class Logger
    {
        private static frmLogger instance;

        public static frmLogger Instance
        {
            get { return instance ?? (instance = new frmLogger()); }
        }
    }
}

using System;

namespace CONetwork.Events
{
    public class SocketConnectionEventArgs:EventArgs
    {
        public Wrapper Wrapper { get; private set; }

        public SocketConnectionEventArgs(Wrapper wr)
        {
            Wrapper = wr;
        }
    }
}

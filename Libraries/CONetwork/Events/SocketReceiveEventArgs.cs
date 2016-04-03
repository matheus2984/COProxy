using System;

namespace CONetwork.Events
{
    public class SocketReceiveEventArgs : EventArgs
    {
        public Wrapper Wrapper { get; private set; }
        public byte[] Data { get; private set; }

        public SocketReceiveEventArgs(Wrapper wr, byte[] data)
        {
            Wrapper = wr;
            Data = data;
        }
    }
}

using System;

namespace CONetwork.Util
{
    public static partial class Extentions
    {
        public static void SafeInvoke(this Delegate eventHandler, object sender, EventArgs args)
        {
            var evt = eventHandler;
            if (evt != null)
                evt.DynamicInvoke(sender, args);
        }

        public static void SafeInvoke<T>(this Delegate handler, object sender, T args)
        {
            var hdl = handler;
            if (hdl != null)
                hdl.DynamicInvoke(sender, args);
        }

        public static void SafeInvoke<T>(this Delegate handler, object sender, T[] args)
        {
            var hdl = handler;
            if (hdl != null)
                hdl.DynamicInvoke(sender, args);
        }
    }
}
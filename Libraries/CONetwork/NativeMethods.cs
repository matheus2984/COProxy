using System.Runtime.InteropServices;

namespace CONetwork
{
    public static unsafe class NativeMethods
    {
        private const string MSVCRT_DLL = @"C:\Windows\system32\msvcrt.dll";
        [DllImport(MSVCRT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* memcpy(void* dst, string value, int length);
    }
}

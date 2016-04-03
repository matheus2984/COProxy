using System;
using System.Runtime.InteropServices;

namespace COCryptography
{
    public static unsafe class NativeExtensions
    {
        public static void CopyTo(this string str, void* pDest)
        {
            var dest = (byte*)pDest;
            for (var i = 0; i < str.Length; i++)
            {
                dest[i] = (byte)str[i];
            }
        }

        public static byte[] UnsafeClone(this byte[] buffer)
        {
            var bufCopy = new byte[buffer.Length];
            fixed (byte* pBuf = buffer, pCopy = bufCopy)
            {
                Native.memcpy(pCopy, pBuf, buffer.Length);
            }
            return bufCopy;
        }
    } 

    public struct SystemTime
    {
        private readonly uint time;

        public SystemTime(uint millis)
        {
            time = millis;
        }

        public SystemTime(int millis)
        {
            time = (uint)millis;
        }

        public SystemTime(long millis)
        {
            time = (uint)millis;
        }

        private SystemTime Add(int value, int scale)
        {
            return AddMilliseconds(value * scale);
        }

        public SystemTime AddMilliseconds(int value)
        {
            return new SystemTime(time + value);
        }

        public SystemTime AddSeconds(int value)
        {
            return Add(value, 1000);
        }

        public SystemTime AddMinutes(int value)
        {
            return Add(value, 60000);
        }

        public SystemTime AddHours(int value)
        {
            return Add(value, 3600000);
        }

        public static SystemTime operator +(SystemTime a, SystemTime b)
        {
            return new SystemTime(a.time + b.time);
        }

        public static SystemTime operator -(SystemTime a, SystemTime b)
        {
            return new SystemTime(a.time - b.time);
        }

        public static bool operator ==(SystemTime a, SystemTime b)
        {
            return a.time == b.time;
        }

        public static bool operator !=(SystemTime a, SystemTime b)
        {
            return a.time != b.time;
        }

        public static bool operator <(SystemTime a, SystemTime b)
        {
            return a.time < b.time;
        }

        public static bool operator >(SystemTime a, SystemTime b)
        {
            return a.time > b.time;
        }

        public static bool operator <=(SystemTime a, SystemTime b)
        {
            return a.time <= b.time;
        }

        public static bool operator >=(SystemTime a, SystemTime b)
        {
            return a.time >= b.time;
        }

        public static implicit operator uint(SystemTime time)
        {
            return time.time;
        }

        public static SystemTime Now
        {
            get
            {
                return Native.timeGetTime();
            }
        }

        public bool Equals(SystemTime other)
        {
            return other.time == time;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(SystemTime)) return false;
            return Equals((SystemTime)obj);
        }

        public override int GetHashCode()
        {
            return time.GetHashCode();
        }

        public override string ToString()
        {
            return time.ToString();
        }
    }
    public unsafe class Native
    {
        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private const string MSVCRT_DLL = @"C:\Windows\system32\msvcrt.dll";
        private const string KERNEL32_DLL = @"kernel32.dll";
        private const string WINMM_DLL = @"winmm.dll";

        public delegate bool ConsoleEventHandler(CtrlType sig);

        [DllImport(MSVCRT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* memcpy(void* dst, void* src, int length);

        [DllImport(MSVCRT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void* memset(void* dst, byte fill, int length);

        [DllImport(KERNEL32_DLL)]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler, bool add);

        [DllImport(WINMM_DLL)]
        public static extern SystemTime timeGetTime();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* memcpy(void* dest, void* src, uint size);
        [DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MessageBox(int h, string m, string c, int type);
        [DllImport("libeay34.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void CAST_set_key(IntPtr _key, int len, byte[] data);

        [DllImport("libeay34.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void CAST_ecb_encrypt(byte[] in_, byte[] out_, IntPtr schedule, int enc);

        [DllImport("libeay34.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void CAST_cbc_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, int enc);

        [DllImport("libeay34.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void CAST_cfb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, ref int num, int enc);

        [DllImport("libeay34.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void CAST_ofb64_encrypt(byte[] in_, byte[] out_, int length, IntPtr schedule, byte[] ivec, out int num);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace COCryptography
{
    public static class ReadWrite
    {
        public static byte[] ReadBytes(byte[] data, int start, int count)
        {
            try
            {
                if (start + count > data.Length)
                    return new byte[0];
                else
                {
                    byte[] info = new byte[count];
                    for (int i = 0; i < count; i++)
                        info[i] = data[i + start];
                    return info;
                }
            }
            catch { return new byte[0]; }
        }
        public static string ReadString(byte[] data, int start, int length)
        {
            try
            {
                return Encoding.ASCII.GetString(ReadBytes(data, start, length));
            }
            catch { return ""; }
        }
        public static void WriteString(string arg, int offset, byte[] buffer)
        {
            try
            {
                if (buffer.Length >= offset + arg.Length)
                {
                    unsafe
                    {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        ushort i = 0;
                        while (i < arg.Length)
                        {
                            *((byte*)(Buffer + offset + i)) = (byte)arg[i];
                            i++;
                        }
                    }
#else
                        ushort i = 0;
                        while (i < arg.Length)
                        {
                            buffer[(ushort)(i + offset)] = (byte)arg[i];
                            i = (ushort)(i + 1);
                        }
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void WriteBool(bool Value, int offset, byte[] buffer)
        {
            try
            {
                if (Value)
                    buffer[offset] = (byte)(1);
                else
                    buffer[offset] = (byte)(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("offset: " + offset + " value " + Value);
                Console.WriteLine(e);
            }

        }
        public static void WriteByte(byte arg, int offset, byte[] buffer)
        {
            try
            {
                buffer[offset] = (byte)(arg);
            }
            catch (Exception e)
            {
                Console.WriteLine("offset: " + offset + " value " + arg);
                Console.WriteLine(e);
            }
        }
        public static void WriteUInt16(ushort arg, int offset, byte[] buffer)
        {
            try
            {
                if (buffer.Length >= offset + sizeof(ushort))
                {
                    unsafe
                    {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        *((ushort*)(Buffer + offset)) = arg;
                    }
#else
                        buffer[offset] = (byte)(arg);
                        buffer[offset + 1] = (byte)(arg >> 8);
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void WriteUInt32(uint arg, int offset, byte[] buffer)
        {
            try
            {
                if (buffer.Length >= offset + sizeof(uint))
                {
                    unsafe
                    {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        *((uint*)(Buffer + offset)) = arg;
                    }
#else
                        buffer[offset] = (byte)(arg);
                        buffer[offset + 1] = (byte)(arg >> 8);
                        buffer[offset + 2] = (byte)(arg >> 16);
                        buffer[offset + 3] = (byte)(arg >> 24);
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void WriteUInt64(ulong arg, int offset, byte[] buffer)
        {
            try
            {
                if (buffer.Length >= offset + sizeof(ulong))
                {
                    unsafe
                    {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        *((ulong*)(Buffer + offset)) = arg;
                    }
#else
                        buffer[offset] = (byte)(arg);
                        buffer[offset + 1] = (byte)(arg >> 8);
                        buffer[offset + 2] = (byte)(arg >> 16);
                        buffer[offset + 3] = (byte)(arg >> 24);
                        buffer[offset + 4] = (byte)(arg >> 32);
                        buffer[offset + 5] = (byte)(arg >> 40);
                        buffer[offset + 6] = (byte)(arg >> 48);
                        buffer[offset + 7] = (byte)(arg >> 56);
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void WriteInt32(int arg, int offset, byte[] buffer)
        {
            if (buffer == null)
                return;
            if (offset > buffer.Length - 1)
                return;
            if (buffer.Length >= offset + sizeof(uint))
            {
                unsafe
                {
#if UNSAFE
                    fixed (byte* Buffer = buffer)
                    {
                        *((int*)(Buffer + offset)) = arg;
                    }
#else
                    buffer[offset] = (byte)(arg);
                    buffer[offset + 1] = (byte)(arg >> 8);
                    buffer[offset + 2] = (byte)(arg >> 16);
                    buffer[offset + 3] = (byte)(arg >> 24);
#endif
                }
            }
        }
        public static void WriteStringList(List<string> arg, int offset, byte[] buffer)
        {
            if (arg == null)
                return;
            if (buffer == null)
                return;
            if (offset > buffer.Length - 1)
                return;
            buffer[offset] = (byte)arg.Count;
            offset++;
            foreach (string str in arg)
            {
                buffer[offset] = (byte)str.Length;
                WriteString(str, offset + 1, buffer);
                offset += str.Length + 1;
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace COCryptography.Cryptography
{
    public enum BlowfishAlgorithm
    {
        ECB,
        CBC,
        CFB64,
        OFB64,
    };

    public class Blowfish : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct bf_key_st
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)] public uint[] P;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] public uint[] S;
        }

        private readonly BlowfishAlgorithm algorithm;
        private readonly IntPtr key;
        private readonly byte[] encryptIv;
        private readonly byte[] decryptIv;
        private int encryptNum;
        private int decryptNum;

        public Blowfish(BlowfishAlgorithm algorithm)
        {
            this.algorithm = algorithm;
            encryptIv = new byte[8] {0, 0, 0, 0, 0, 0, 0, 0};
            decryptIv = new byte[8] {0, 0, 0, 0, 0, 0, 0, 0};
            bf_key_st bfKeySt = new bf_key_st();
            bfKeySt.P = new uint[16 + 2];
            bfKeySt.S = new uint[4*256];
            key = Marshal.AllocHGlobal(bfKeySt.P.Length*sizeof (uint) + bfKeySt.S.Length*sizeof (uint));
            Marshal.StructureToPtr(bfKeySt, key, false);
            encryptNum = 0;
            decryptNum = 0;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(key);
        }

        public void SetKey(byte[] data)
        {
            encryptNum = 0;
            decryptNum = 0;
            Native.CAST_set_key(key, data.Length, data);
        }

        public byte[] Encrypt(byte[] buffer)
        {
            byte[] ret = new byte[buffer.Length];
            switch (algorithm)
            {
                case BlowfishAlgorithm.ECB:
                    Native.CAST_ecb_encrypt(buffer, ret, key, 1);
                    break;
                case BlowfishAlgorithm.CBC:
                    Native.CAST_cbc_encrypt(buffer, ret, buffer.Length, key, encryptIv, 1);
                    break;
                case BlowfishAlgorithm.CFB64:
                    Native.CAST_cfb64_encrypt(buffer, ret, buffer.Length, key, encryptIv, ref encryptNum, 1);
                    break;
                case BlowfishAlgorithm.OFB64:
                    Native.CAST_ofb64_encrypt(buffer, ret, buffer.Length, key, encryptIv, out encryptNum);
                    break;
            }
            return ret;
        }

        public byte[] Decrypt(byte[] buffer)
        {
            byte[] ret = new byte[buffer.Length];
            switch (algorithm)
            {
                case BlowfishAlgorithm.ECB:
                    Native.CAST_ecb_encrypt(buffer, ret, key, 0);
                    break;
                case BlowfishAlgorithm.CBC:
                    Native.CAST_cbc_encrypt(buffer, ret, buffer.Length, key, decryptIv, 0);
                    break;
                case BlowfishAlgorithm.CFB64:
                    Native.CAST_cfb64_encrypt(buffer, ret, buffer.Length, key, decryptIv, ref decryptNum, 0);
                    break;
                case BlowfishAlgorithm.OFB64:
                    Native.CAST_ofb64_encrypt(buffer, ret, buffer.Length, key, decryptIv, out decryptNum);
                    break;
            }
            return ret;
        }

        public byte[] EncryptIV
        {
            get { return encryptIv; }
            set { Buffer.BlockCopy(value, 0, encryptIv, 0, 8); }
        }

        public byte[] DecryptIV
        {
            get { return decryptIv; }
            set { Buffer.BlockCopy(value, 0, decryptIv, 0, 8); }
        }
    }
}
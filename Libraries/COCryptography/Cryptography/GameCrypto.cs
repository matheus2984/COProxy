using System;
using OpenSSL;

namespace COCryptography.Cryptography
{
    public class GameCrypto
    {
        private readonly Blowfish blowfish;
        public DH Dh;

        public GameCrypto(byte[] key)
        {
            blowfish = new Blowfish(BlowfishAlgorithm.CFB64);
            blowfish.SetKey(key);
        }

        public void Decrypt(byte[] packet)
        {
            byte[] buffer = blowfish.Decrypt(packet);
            Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
        }

        public void Encrypt(byte[] packet)
        {
            byte[] buffer = blowfish.Encrypt(packet);
            Buffer.BlockCopy(buffer, 0, packet, 0, buffer.Length);
        }

        public Blowfish Blowfish
        {
            get { return blowfish; }
        }
    }
}

namespace COCryptography.Cryptography
{
    public class AuthProtocolCryptographer
    {
        private class CryptCounter
        {
            private ushort mCounter;

            public byte Key2
            {
                get { return (byte)(mCounter >> 8); }
            }

            public byte Key1
            {
                get { return (byte)(mCounter & 0xFF); }
            }

            public void Increment()
            {
                mCounter++;
            }
        }

        private readonly CryptCounter decryptCounter;
        private readonly CryptCounter encryptCounter;
        private readonly byte[] cryptKey1;
        private readonly byte[] cryptKey2;

        public AuthProtocolCryptographer()
        {
            decryptCounter = new CryptCounter();
            encryptCounter = new CryptCounter();
            cryptKey1 = new byte[0x100];
            cryptKey2 = new byte[0x100];
            byte iKey1 = 0x9D;
            byte iKey2 = 0x62;
            for (int i = 0; i < 0x100; i++)
            {
                cryptKey1[i] = iKey1;
                cryptKey2[i] = iKey2;
                iKey1 = (byte)((0x0F + (byte)(iKey1 * 0xFA)) * iKey1 + 0x13);
                iKey2 = (byte)((0x79 - (byte)(iKey2 * 0x5C)) * iKey2 + 0x6D);
            }
        }

        public void Decrypt(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)0xAB;
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)(cryptKey1[decryptCounter.Key1] ^ cryptKey2[decryptCounter.Key2]);
                decryptCounter.Increment();
            }
        }
        public void Encrypt(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)0xAB;
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)(cryptKey1[encryptCounter.Key1] ^ cryptKey2[encryptCounter.Key2]);
                encryptCounter.Increment();
            }
        }

        public void EncryptBackwards(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)(cryptKey2[encryptCounter.Key2] ^ cryptKey1[encryptCounter.Key1]);
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)0xAB;

                encryptCounter.Increment();
            }
        }
        public void DecryptBackwards(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= (byte)(cryptKey2[decryptCounter.Key2] ^ cryptKey1[decryptCounter.Key1]);
                buffer[i] = (byte)(buffer[i] >> 4 | buffer[i] << 4);
                buffer[i] ^= (byte)0xAB;

                decryptCounter.Increment();
            }
        }

        public void GenerateKeys(uint cryptoKey, uint accountId)
        {
            uint tmpkey1 = 0, tmpkey2 = 0;
            tmpkey1 = ((cryptoKey + accountId) ^ (0x4321)) ^ cryptoKey;
            tmpkey2 = tmpkey1 * tmpkey1;

            for (int i = 0; i < 256; i++)
            {
                int right = ((3 - (i % 4)) * 8);
                int left = ((i % 4)) * 8 + right;
                cryptKey1[i] ^= (byte)(tmpkey1 << right >> left);
                cryptKey2[i] ^= (byte)(tmpkey2 << right >> left);
            }
        }
    }
}


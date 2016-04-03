using System;
using System.Text;

namespace COPacketLibrary
{
    public abstract unsafe class Packet<T> where T : Packet<T>
    {
        private const int MAX_SIZE = 1024;
        private byte[] Data { get; set; }
        private int offset;

        private int Offset
        {
            get { return offset; }
            set
            {
                offset = value;
            }
        }

        public ushort Length { get; set; }
        public ushort Type { get; set; }
        public string Seal { get; set; }

        public Packet(byte[] data)
        {
            Data = data;
            Length = ReadUShort();
            Type = ReadUShort();
            Offset = data.Length - 8;
            Seal = ReadString(8);
            Offset = 2;
        }

        public Packet(ushort type, string seal)
        {
            Data = new byte[MAX_SIZE];
            Type = type;
            Seal = seal;
        }

        public int ReadInt()
        {
            int value;
            fixed (byte* packet = Data)
                value = *(int*) (packet + Offset);

            Offset += sizeof (int);
            return value;
        }

        public uint ReadUInt()
        {
            uint value;
            fixed (byte* packet = Data)
                value = *(uint*) (packet + Offset);

            Offset += sizeof (uint);
            return value;
        }

        public short ReadShort()
        {
            short value;
            fixed (byte* packet = Data)
                value = *(short*) (packet + Offset);

            Offset += sizeof (short);
            return value;
        }

        public ushort ReadUShort()
        {
            ushort value;
            fixed (byte* packet = Data)
                value = *(ushort*) (packet + Offset);

            Offset += sizeof (ushort);
            return value;
        }

        public long ReadLong()
        {
            long value;
            fixed (byte* packet = Data)
                value = *(long*) (packet + Offset);

            Offset += sizeof (long);
            return value;
        }

        public ulong ReadULong()
        {
            ulong value;
            fixed (byte* packet = Data)
                value = *(ulong*) (packet + Offset);

            Offset += sizeof (ulong);
            return value;
        }

        public byte ReadByte()
        {
            var data = Data[Offset];
            Offset += sizeof (byte);
            return data;
        }

        public bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        public string ReadString(int length)
        {
            var data = Encoding.UTF8.GetString(Data, Offset, length);
            Offset += data.Length;
            return data;
        }

        public void Write(int value)
        {
            fixed (byte* packet = Data)
                *(int*) (packet + Offset) = value;

            Offset += sizeof (int);
        }

        public void Write(uint value)
        {
            fixed (byte* packet = Data)
                *(uint*) (packet + Offset) = value;

            Offset += sizeof (uint);
        }

        public void Write(short value)
        {
            fixed (byte* packet = Data)
                *(short*) (packet + Offset) = value;

            Offset += sizeof (short);
        }

        public void Write(ushort value)
        {
            fixed (byte* packet = Data)
                *(ushort*) (packet + Offset) = value;

            Offset += sizeof (ushort);
        }

        public void Write(long value)
        {
            fixed (byte* packet = Data)
                *(long*) (packet + Offset) = value;

            Offset += sizeof (long);
        }

        public void Write(ulong value)
        {
            fixed (byte* packet = Data)
                *(ulong*) (packet + Offset) = value;

            Offset += sizeof (ulong);
        }

        public void Write(byte value)
        {
            Data[Offset] = value;
            Offset += sizeof (byte);
        }

        public void Write(bool value)
        {
            Write((byte) (value ? 1 : 0));
        }

        public void Write(string value)
        {
            fixed (byte* packet = Data)
                NativeMethods.memcpy(packet + Offset, value, value.Length);

            Offset += value.Length;
        }

        public void Seek(int position)
        {
            if (position <= MAX_SIZE)
                Offset = position;
        }

        public void LeftShiftResize(int count)
        {
            var newData = new byte[Data.Length - count];

            Buffer.BlockCopy(Data, 0, newData, 0, offset);
            Buffer.BlockCopy(Data, offset+count, newData, offset, (Data.Length - offset));
            Data = newData;
        }

        public void RightShiftResize(int count)
        {
            var newData = new byte[Data.Length + count];

            Buffer.BlockCopy(Data, 0, newData, 0, offset);
            Buffer.BlockCopy(Data, offset, newData, offset + count, (Data.Length - offset));
            Data = newData;
        }

        public byte[] GetData()
        {

            byte[] dataOut;

            if (Data.Length == MAX_SIZE)
            {
                Write(Seal);
                ushort length = (ushort)offset;

                Seek(0);
                Write(length-8);
                Write(Type);    

                dataOut = new byte[length];
                Buffer.BlockCopy(Data, 0, dataOut, 0, dataOut.Length);
            }
            else
            {
                Seek(0);
                Write(Convert.ToUInt16(Data.Length-8));
                dataOut = Data;
            }

            return dataOut;
        }

        public byte[] ReGetData()
        {
            return Data;
        }

        public abstract byte[] Serialize();
        public abstract T Deserialize();
    }
}
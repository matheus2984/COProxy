using System;

namespace CONetwork.Util
{
    public class PacketQueue
    {
        private byte[] packets = new byte[ushort.MaxValue];
        private int stackPointer;

        public void Push(byte[] data)
        {
            Buffer.BlockCopy(data, 0, packets, stackPointer, data.Length);
            stackPointer += data.Length;
        }

        public bool CanPop()
        {
            return stackPointer > 4 && BitConverter.ToUInt16(packets, 0) + 8 <= stackPointer;
        }

        public byte[] Pop()
        {
            var tobePoped = new byte[BitConverter.ToUInt16(packets, 0) + 8];
            Buffer.BlockCopy(packets, 0, tobePoped, 0, BitConverter.ToUInt16(packets, 0) + 8);
            var updatePackets = new byte[ushort.MaxValue];
            Buffer.BlockCopy(packets, tobePoped.Length, updatePackets, 0, packets.Length - tobePoped.Length);
            packets = updatePackets;
            if (stackPointer >= tobePoped.Length)
            {
                stackPointer -= tobePoped.Length;
            }
            else
            {
                Console.WriteLine("TobePoped" + tobePoped.Length + "," + "StackPointer" + stackPointer);
                stackPointer = 0;
                packets = new byte[ushort.MaxValue];
            }
            return tobePoped;
        }
    }
}
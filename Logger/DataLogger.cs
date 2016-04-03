using System;
using System.Text;

namespace Logger
{
    public class DataLogger
    {
        public ushort Size { get; private set; }
        public ushort Type { get; private set; }
        public byte[] Data { get; private set; }
        public string Seal { get; private set; }
     
        public DataLogger(byte[] data)
        {
            Data = data;
            Size = BitConverter.ToUInt16(data, 0);
            Type = BitConverter.ToUInt16(data, 2);
            Seal = Encoding.UTF8.GetString(data, data.Length - 8, 8);
        }

        public override string ToString()
        {
            return BitConverter.ToString(Data).Replace('-', ' ');
        }
    }
}
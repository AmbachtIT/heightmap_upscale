using System;
using System.IO;

namespace Ambacht.Data
{
    public static class Extensions
    {

        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}

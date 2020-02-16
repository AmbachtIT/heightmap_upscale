using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace Ambacht.Data.Mnist
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///[offset] [type]          [value]          [description]
    /// 0000     32 bit integer  0x00000801(2049) magic number(MSB first
    /// 0004     32 bit integer  60000            number of item
    /// 0008     unsigned byte   ??               label
    /// 0009     unsigned byte   ??               label
    /// </remarks>
    public class MnistLabels
    {

        public static MnistLabels Read(string path)
        {
            Stream stream = File.OpenRead(path);
            if (path.EndsWith(".gz"))
            {
                stream = new GZipInputStream(stream);
            }

            using (var reader = new BinaryReader(stream))
            {
                var magic = reader.ReadInt32BigEndian();
                if(magic != 0x801)
                {
                    throw new InvalidOperationException();
                }

                var count = reader.ReadInt32BigEndian();
                var result = new MnistLabels();
                for (var i = 0; i < count; i++)
                {
                    result.labels.Add(reader.ReadByte());
                }

                return result;
            }
        }

        private readonly List<byte> labels = new List<byte>();

    }
}

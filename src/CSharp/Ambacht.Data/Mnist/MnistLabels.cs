using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using NumSharp;
using NumSharp.Generic;

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
                var result = new MnistLabels()
                {
                    Count = count,
                    Labels = new NDArray<byte>(new Shape(count))
                };
                for (var i = 0; i < count; i++)
                {
                    result.Labels[i] = reader.ReadByte();
                }

                return result;
            }
        }

        public int Count { get; set; }
        
        public NDArray<byte> Labels { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
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
    /// [offset] [type]          [value]          [description]
    /// 0000     32 bit integer  0x00000803(2051) magic number
    /// 0004     32 bit integer  60000            number of images
    /// 0008     32 bit integer  28               number of rows
    /// 0012     32 bit integer  28               number of columns
    /// 0016     unsigned byte   ??               pixel
    /// 0017     unsigned byte   ??               pixel
    /// </remarks>
    public class MnistImages
    {

        public static MnistImages Read(string path)
        {
            Stream stream = File.OpenRead(path);
            if (path.EndsWith(".gz"))
            {
                stream = new GZipInputStream(stream);
            }

            using (var reader = new BinaryReader(stream))
            {
                var magic = reader.ReadInt32BigEndian();
                if(magic != 0x803)
                {
                    throw new InvalidOperationException();
                }

                var count = reader.ReadInt32BigEndian();
                var rows = reader.ReadInt32BigEndian();
                var columns = reader.ReadInt32BigEndian();
                var result = new MnistImages()
                {
                    Count = count,
                    Images = new NDArray<byte>(new Shape(count, rows, columns))
                };
                
                for (var i = 0; i < count; i++)
                {
                    for (var r = 0; r < rows; r++)
                    {
                        for (var c = 0; c < columns; c++)
                        {
                            var b = reader.ReadByte();
                            result.Images[i, r, c] = b;
                        }
                    }
                }

                return result;
            } 
        }

        public int Count { get; set; }
        public NDArray<byte> Images { get; set; }

    }
}

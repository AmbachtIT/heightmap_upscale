using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NumSharp;
using NumSharp.Generic;

namespace Ambacht.Data.Dem
{
    public static class NzReader
    {
        
        public static Heightmap Read(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return Read(stream);
            }            
        }

        public static Heightmap Read(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var result = ReadHeader(reader);
                for (var r = 0; r < result.Rows; r++)
                {
                    var line = reader.ReadLine();
                    var parts = Tokenize(line).ToList();
                    for (var c = 0; c < result.Columns; c++)
                    {
                        var value = ParseFloat(parts[c]);
                        result.Data[r, c] = value;
                    }
                }

                return result;
            }
        }

        private static Heightmap ReadHeader(StreamReader reader)
        {
            var result = new Heightmap();
            result.Columns = int.Parse(ReadHeader(reader, "ncols"));
            result.Rows = int.Parse(ReadHeader(reader, "nrows"));
            ReadHeader(reader, "xllcorner");
            ReadHeader(reader, "yllcorner");
            result.CellSize = ParseFloat(ReadHeader(reader, "cellsize"));
            result.NoDataValue = ParseFloat(ReadHeader(reader, "NODATA_value"));
            result.Data = new NDArray<float>(new Shape(result.Rows, result.Columns));
            return result;
        }

        private static float ParseFloat(string value)
        {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static string ReadHeader(StreamReader reader, string header)
        {
            var line = reader.ReadLine();
            var parts = Tokenize(line).ToList();
            if (parts.Count != 2)
            {
                throw new InvalidOperationException();
            }
            if (parts[0] != header)
            {
                throw new InvalidOperationException();
            }

            return parts[1];
        }

        private static IEnumerable<string> Tokenize(string line)
        {
            var builder = new StringBuilder();
            foreach (var c in line)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (builder.Length != 0)
                    {
                        yield return builder.ToString();
                        builder.Clear();
                    }
                }
                else
                {
                    builder.Append(c);
                }
            }
            if (builder.Length != 0)
            {
                yield return builder.ToString();
            }
        }
    }
}
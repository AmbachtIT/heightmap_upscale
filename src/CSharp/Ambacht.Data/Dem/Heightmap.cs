using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NumSharp;
using NumSharp.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Ambacht.Data.Dem
{
    public class Heightmap
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public float CellSize { get; set; }

        public float NoDataValue { get; set; }
        public NDArray<float> Data { get; set; }

        public float this[int row, int column]
        {
            get => Data[row, column];
            set => Data[row, column] = value;
        }

        public void ReplaceNoDataValue(float newValue)
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                {
                    if (Data[r, c] == NoDataValue)
                    {
                        Data[r, c] = newValue;
                    }
                }
            }
        }

        

        public void Write(BinaryWriter writer)
        {
            writer.Write(Rows);
            writer.Write(Columns);
            writer.Write(CellSize);
            writer.Write(NoDataValue);
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                {
                    writer.Write(Data[r, c]);
                }
            }
        }
        
    }

    public static class HeightmapExtensions
    {
        /// <summary>
        /// Returns a copy with half the resolution of this image.
        /// </summary>
        /// <returns></returns>
        public static NDArray<float> Downscale(this NDArray<float> arr, int factor, float? noDataValue = null)
        {
            var height = arr.shape[0];
            var width = arr.shape[1];
            var result = new NDArray<float>(new Shape(height / factor, width / factor));
            for (var r = 0; r < result.shape[0]; r++)
            {
                for (var c = 0; c < result.shape[1]; c++)
                {
                    var valid = 0;
                    var sum = 0f;
                    for (var r1 = 0; r1 < factor; r1++)
                    {
                        for (var c1 = 0; c1 < factor; c1++)
                        {
                            var value = arr[r * factor + r1, c * factor + c1];
                            if (value != noDataValue)
                            {
                                sum += value;
                                valid++;
                            }
                        }
                    }

                    if (valid == 0)
                    {
                        result[r, c] = noDataValue.Value;
                    }
                    else
                    {
                        result[r, c] = sum / valid;
                    }
                }
            }

            return result;
        }

        public static Range Range(this NDArray<float> arr)
        {
            return new Range()
            {
                Min = arr.min<float>(),
                Max = arr.max<float>()
            };
        }
    }

    public class Range
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public float Delta => Max - Min;
    }
}
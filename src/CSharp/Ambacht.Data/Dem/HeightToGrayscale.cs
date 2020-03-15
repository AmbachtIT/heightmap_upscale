using System;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using NumSharp.Generic;
using SixLabors.ImageSharp.PixelFormats;

namespace Ambacht.Data.Dem
{
    public static class HeightToGrayscale
    {

        public static Func<float, Gray16> FullRange16(Heightmap heightmap)
        {
            var min = heightmap.Data.min<float>();
            var max = heightmap.Data.max<float>();
            if (min == max)
            {
                return v => new Gray16();
            }

            var delta = max - min;

            return v =>
            {
                v -= min;
                v /= delta;
                v *= 65535;
                return new Gray16((ushort)v);
            };
        }

        public static Func<float, Gray8> FullRange8(NDArray<float> map)
        {
            var min = map.min<float>();
            var max = map.max<float>();
            if (min == max)
            {
                return v => new Gray8();
            }

            var delta = max - min;

            return v =>
            {
                v -= min;
                v /= delta;
                v *= 255;
                return new Gray8((byte)v);
            };
        }

        public static Func<float, Gray16> Unit16()
        {
            return v =>
            {
                v *= 65535;
                return new Gray16((ushort)v);
            };
        }


        public static Func<float, Gray8> Unit8()
        {
            return v =>
            {
                v *= 255;
                return new Gray8((byte)v);
            };
        }

    }
}

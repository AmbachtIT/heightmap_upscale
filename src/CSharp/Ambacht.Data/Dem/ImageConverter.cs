using System;
using System.IO;
using NumSharp;
using NumSharp.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Ambacht.Data.Dem
{
    public class ImageConverter
    {
        public Image<TPixel> CreateImage<TPixel>(NDArray<float> map, Func<float, TPixel> getColor) where TPixel: struct, IPixel<TPixel>
        {
            var image = new Image<TPixel>(map.shape[1], map.shape[0]);
            for (var r = 0; r < map.shape[1]; r++)
            {
                for (var c = 0; c < map.shape[0]; c++)
                {
                    var value = map[r, c];
                    image[c, r] = getColor(value);
                }
            }
            return image;
        }
        
        public void ToPng<TPixel>(NDArray<float> map, Stream stream, Func<float, TPixel> getColor) where TPixel: struct, IPixel<TPixel>
        {
            using (var image = CreateImage(map, getColor))
            {
                image.SaveAsPng(stream, new PngEncoder()
                {
                    ColorType = PngColorType.Grayscale,
                    BitDepth = GetBitDepth<TPixel>(),
                    
                });
            }
        }
        
        private PngBitDepth GetBitDepth<TPixel>() where TPixel : struct, IPixel<TPixel>
        {
            if (typeof(TPixel) == typeof(Gray16))
            {
                return PngBitDepth.Bit16;
            }

            if (typeof(TPixel) == typeof(Gray8))
            {
                return PngBitDepth.Bit8;
            }
            throw new InvalidOperationException();
        }
    }
}
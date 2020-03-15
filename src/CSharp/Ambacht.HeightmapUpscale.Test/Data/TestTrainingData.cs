using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ambacht.Data.Common;
using Ambacht.Data.Dem;
using NumSharp;
using NumSharp.Generic;
using NUnit.Framework;

namespace Ambacht.HeightmapUpscale.Test.Data
{
    public class TestTrainingData
    {
        [Test(), Explicit()]
        public void All()
        {
            ExtractOriginals();
            ExtractTrainingData();
        }


        [Test(), Explicit()]
        public void ExtractOriginals()
        {
            foreach (var tile in TileCodes)
            {
                var path = $@"D:\Data\DEM\NZ\{tile}.asc";
                var file = NzReader.Read(path);
                Assert.AreEqual(8192, file.Columns);
                Assert.AreEqual(8192, file.Rows);
                Assert.AreEqual(-999, file.NoDataValue);

                var list = new StatList();
                var arr = file.Data;
                foreach (var index in arr.Indices())
                {
                    var value = arr[index];
                    if (value == file.NoDataValue)
                    {
                        foreach (var neighbour in arr.Neighbours(index))
                        {
                            var neighbourValue = arr[neighbour];
                            if (neighbourValue != file.NoDataValue)
                            {
                                list.Add(neighbourValue);
                            }
                        }
                    }
                }

                if (list.Any())
                {
                    file.ReplaceNoDataValue(list.Average());
                }

                var outputPath = Path.Combine(OutputPath, $"{tile}.npy");
                np.save(outputPath, file.Data);
                using (var stream = File.Create(Path.Combine(OutputPath, $"{tile}.png")))
                {
                    new ImageConverter().ToPng(file.Data, stream, HeightToGrayscale.FullRange8(file.Data));
                }
            }
        }

        [Test(), Explicit()]
        public void ExtractTrainingData()
        {
            int count = 0;
            foreach (var fullCode in TileCodes)
            {
                var full = np.load(Path.Combine(OutputPath, $"{fullCode}.npy")).AsGeneric<float>();
                var fullRange = full.Range();
                foreach (var tile in GetYTiles(full))
                {
                    var tileRange = tile.Range();
                    if (count % 11 < 2 && tileRange.Delta > 1) 
                    {
                        // Don't use tiles with a very small range
                        var set = count % 7 == 0 ? "test" : "train";
                        var range = count % 2 == 0 ? fullRange : tileRange;

                        Normalize(tile, range);
                        

                        np.save(Path.Combine(OutputPath, $"{set}_y", $"sample-{count:00000}.npy"), tile);
                        using (var stream = File.Create(Path.Combine(OutputPath, $"{set}_y", $"sample-{count:00000}.png")))
                        {
                            new ImageConverter().ToPng(tile, stream, HeightToGrayscale.Unit8());
                        }

                        var x =
                            tile
                                .Downscale(Scale);
                        if (count % 4 < 2)
                        {
                             Downsample(x, 255);

                        }
                        else
                        {
                             Downsample(x, 65535);
                        }

                        np.save(Path.Combine(OutputPath, $"{set}_x", $"sample-{count:00000}.npy"), x);
                        using (var stream = File.Create(Path.Combine(OutputPath, $"{set}_x", $"sample-{count:00000}.png")))
                        {
                            new ImageConverter().ToPng(x, stream, HeightToGrayscale.Unit8());
                        }



                    }
                    count++;
                }

            }

        }


        [Test(), Explicit()]
        public void VisualizeResults()
        {
            foreach (var path in Directory.GetFiles(Path.Combine(OutputPath, "predicted"), "*.npy"))
            {
                var info = new FileInfo(path);
                var predicted = np.load(info.FullName).AsGeneric<float>();
                using (var stream = File.Create(Path.Combine(OutputPath, "Predicted", info.Name.Replace(".npy", ".png"))))
                {
                    new ImageConverter().ToPng(predicted, stream, HeightToGrayscale.Unit8());
                }

                var original = np.load(Path.Combine(OutputPath, "test_y", info.Name)).AsGeneric<float>();
                
            }
        }

        private void Normalize(NDArray<float> arr, Ambacht.Data.Dem.Range range)
        {
            for (var r = 0; r < arr.Shape[0]; r++)
            {
                for (var c = 0; c < arr.Shape[1]; c++)
                {
                    arr[r, c] = (arr[r, c] - range.Min) / range.Delta;
                }
            }
        }

        private void Downsample(NDArray<float> arr, float steps)
        {
            for (var r = 0; r < arr.Shape[0]; r ++)
            {
                for (var c = 0; c < arr.Shape[1]; c ++)
                {
                    arr[r, c] = (float) (Math.Round(arr[r, c] * steps) / steps);
                }
            }
        }

        private IEnumerable<NDArray<float>> GetYTiles(NDArray<float> tile)
        {
            for (var r = 0; r < tile.Shape[0]; r+=TileSize)
            {
                for (var c = 0; c < tile.Shape[1]; c+=TileSize)
                {
                    yield return tile[$"{r}:{r + TileSize},{c}:{c + TileSize}"].AsGeneric<float>();
                }
            }
        }


        private const int Scale = 4;
        private readonly int TileSize = 128;
        private readonly string[] TileCodes = new[] {"BJ", "RF", "UB", "UC" };
        private readonly string OutputPath = $@"D:\Data\heightmap_upscale\{Scale}x";
    }
}
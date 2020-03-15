using System;
using System.IO;
using System.Linq;
using Ambacht.Data.Common;
using Ambacht.Data.Dem;
using NUnit.Framework;
using SixLabors.ImageSharp.PixelFormats;

namespace Ambacht.HeightmapUpscale.Test.Data
{
    
    [TestFixture()]
    public class TestNzDem
    {

        [Test(), Explicit()]
        public void LoadFile()
        {
            foreach (var tile in new[] { "BJ", "UB", "UC" })
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
            
                file.ReplaceNoDataValue(list.Average());


                using (var stream = File.Create($@"D:\Data\DEM\NZ\{tile}.png"))
                {
                    new ImageConverter().ToPng(file.Data, stream, HeightToGrayscale.FullRange8(file.Data));
                }
            }
        }
        
    }
}
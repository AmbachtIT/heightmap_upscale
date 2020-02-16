using System;
using System.Collections.Generic;
using System.Text;
using Ambacht.Data.Mnist;
using NUnit.Framework;

namespace Ambacht.HeightmapUpscale.Test.Data
{

    [TestFixture()]
    public class TestMnist
    {

        [Test()]
        public void TestRead()
        {
            var set = MnistDataset.Read(@"C:\Projects\heightmap_upscale\data");
        }

    }
}

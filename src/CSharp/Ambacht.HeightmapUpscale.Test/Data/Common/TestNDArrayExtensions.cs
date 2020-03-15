using System;
using System.Collections.Generic;
using System.Linq;
using Ambacht.Data.Common;
using NumSharp;
using NumSharp.Generic;
using NUnit.Framework;

namespace Ambacht.HeightmapUpscale.Test.Data.Common
{
    
    [TestFixture()]
    public class TestNDArrayExtensions
    {
        
        [Test(), TestCaseSource(nameof(TestCases))]
        public void TestIndices(NDArray arr)
        {
            var total = 0f;
            foreach (var i in arr.Indices())
            {
                total += arr[i];
            }
            Assert.AreEqual(arr.sum(), total);
        }

        [Test(), TestCaseSource(nameof(TestCases))]
        public void TestNeighbours(NDArray arr)
        {
            var origin = arr.Indices().First();
            var neighbours = arr.Neighbours(origin).ToList();
            var dimensionsLargerThan1 = arr.shape.Count(n => n > 1);
            Assert.AreEqual((int)Math.Pow(2, dimensionsLargerThan1) - 1, neighbours.Count);
        }

        
        private static IEnumerable<NDArray> TestCases()
        {
            yield return np.array(new[]
            {
                new float[] {0}
            });
            yield return np.array(new[]
            {
                new float[] {  0, 1, 2 }
            });
            yield return np.array(new[]
            {
                new float[] {  0, 1, 2 },
                new float[] {  4, 5, 6 },
            });
            yield return np.array(new[]
            {
                new[]
                {
                    new float[] {  0, 1, 2 },
                    new float[] {  4, 5, 6 },
                },
                new[]
                {
                    new float[] {  10, 21, 32 },
                    new float[] {  44, 55, 66 },
                },
            });
        }
    }
}
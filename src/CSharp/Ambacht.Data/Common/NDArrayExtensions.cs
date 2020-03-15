using System;
using System.Collections.Generic;
using System.Transactions;
using NumSharp;

namespace Ambacht.Data.Common
{
    public static class NDArrayExtensions
    {

        public static IEnumerable<int[]> Indices(this NDArray arr)
        {
            var shape = arr.Shape;
            var result = new int[shape.NDim];
            foreach (var r in Indices(result, shape, 0))
            {
                yield return r;
            }
        }
        
        private static IEnumerable<int[]> Indices(int[] result, Shape shape, int dim)
        {
            if (dim == shape.NDim)
            {
                yield return result;
            }
            else
            {
                for (var i = 0; i < shape[dim]; i++)
                {
                    result[dim] = i;
                    foreach (var r in Indices(result, shape, dim + 1))
                    {
                        yield return r;
                    }
                }
            }            
        }
        
        public static IEnumerable<int[]> Neighbours(this NDArray arr, params int[] index)
        {
            var shape = arr.Shape;
            var result = new int[shape.NDim];
            foreach (var r in Neighbours(result, shape, 0, index))
            {
                yield return r;
            }
        }
        
        private static IEnumerable<int[]> Neighbours(int[] result, Shape shape, int dim, params int[] index)
        {
            if (dim == shape.NDim)
            {
                if (!IsSelf(result, index))
                {
                    yield return result;
                }
            }
            else
            {
                var ifrom = Math.Max(index[dim] - 1, 0);
                var ito = Math.Min(index[dim] + 1, shape[dim] - 1);
                for (var i = ifrom; i <= ito; i++)
                {
                    result[dim] = i;
                    foreach (var r in Neighbours(result, shape, dim + 1, index))
                    {
                        yield return r;
                    }
                }
            }            
        }

        private static bool IsSelf(int[] result, int[] index)
        {
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] != index[i])
                {
                    return false;
                }
            }
            return true;
        }
        
        private static bool IsDiagonal(int[] result, int[] index)
        {
            throw new NotImplementedException();
        }

    }
}
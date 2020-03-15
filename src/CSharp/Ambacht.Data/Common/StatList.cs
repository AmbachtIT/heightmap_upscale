using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambacht.Data.Common
{
    public class StatList : List<float>
    {
        public float Variance()
        {
            if (Count == 0)
            {
                return 0;
            }
            var avg = this.Average();
            var ss = 0f;
            foreach (var value in this)
            {
                var error = value - avg;
                ss += error * error;
            }

            return ss / Count;
        }

        public float StandardDeviation()
        {
            return (float)Math.Sqrt(Variance());
        }

        public override string ToString()
        {
            return $"M = {this.Average():0.00}, sd = {this.StandardDeviation():0.00}";
        }
    }
}
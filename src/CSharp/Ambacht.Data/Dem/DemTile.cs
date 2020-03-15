using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NumSharp;
using NumSharp.Generic;

namespace Ambacht.Data.NzDem
{
    public class NzFile
    {
        public int Columns { get; set; }

        public int Rows { get; set; }

        public float CellSize { get; set; }

        public float NoDataValue { get; set; }
        public NDArray<float> Data { get; set; }
        
        
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Ambacht.Data
{
    public class Matrix
    {
        public Matrix(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.data = new float[rows, columns];
        }


        public int Rows { get; }
        public int Columns { get; }

        private readonly float[,] data;


        public float this[int row, int column]
        {
            get { return data[row, column]; }
            set { data[row, column] = value; }
        }

        public void Divide(float value)
        {
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Columns; c++)
                {
                    data[r, c] /= value;
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ambacht.Data.Mnist
{
    public class MnistDataset
    {

        public static MnistDataset Read(string path)
        {
            return new MnistDataset()
            {
                TrainingLabels = MnistLabels.Read(Path.Combine(path, "train-labels-idx1-ubyte.gz")),
                TrainingImages = MnistImages.Read(Path.Combine(path, "train-images-idx3-ubyte.gz")),
                TestLabels = MnistLabels.Read(Path.Combine(path, "t10k-labels-idx1-ubyte.gz")),
                TestImages = MnistImages.Read(Path.Combine(path, "t10k-images-idx3-ubyte.gz")),
            };
        }

        public MnistLabels TrainingLabels { get; private set; }
        public MnistImages TrainingImages { get; private set; }
        public MnistLabels TestLabels { get; private set; }
        public MnistImages TestImages { get; private set; }


    }
}

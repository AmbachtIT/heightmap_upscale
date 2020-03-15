using System;
using System.Collections.Generic;
using System.Text;
using Ambacht.Data.Mnist;
using NumSharp;
using NumSharp.Generic;
using NUnit.Framework;
using Tensorflow.Keras.Engine;
using Tensorflow.Layers;
using Tensorflow.Operations.Activation;


namespace Ambacht.HeightmapUpscale.Test.Book
{

    [TestFixture()]
    public class Mnist
    {

        [Test()]
        public void Test1()
        {
            var epochs = 200;
            var batchSize = 128;
            var classes = 10;
            var hiddenCount = 128;
            var validationSplit = 0.2;
            var flatImageSize = 28 * 28;

            var mnist = MnistDataset.Read(@"C:\Projects\heightmap_upscale\data\mnist");

            var x_train = PrepareImage(mnist.TrainingImages);
            var x_test = PrepareImage(mnist.TestImages);

            var y_train = PrepareLabels(mnist.TrainingLabels);
            
            var model = new Sequential();
            model.add(new Dense(classes, activation: new softmax()));
            //model.Compile("SGD", loss: "categorical_crossentropy", metrics: new[] { "accuracy" });

            model.compile("SGD", "categorical_crossentropy");
            
        }

        private NDArray<float> PrepareLabels(MnistLabels labels)
        {
            
            throw new NotImplementedException();
        }

        private NDArray<float> PrepareImage(MnistImages images)
        {
            var shape = images.Images.Shape;
            var size = shape[1] *shape[2];
            return
                images
                    .Images
                    .reshape(images.Count, size)
                    .AsGeneric<float>();
        }

    }
}

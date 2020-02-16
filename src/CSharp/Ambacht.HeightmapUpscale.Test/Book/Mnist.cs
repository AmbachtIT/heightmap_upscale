using System;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using NUnit.Framework;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
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

            var model = new Sequential();
            model.add(new Dense(classes, activation: new softmax()));
            //model.Compile("SGD", loss: "categorical_crossentropy", metrics: new[] { "accuracy" });

            model.compile("SGD", "categorical_crossentropy");
        }

    }
}

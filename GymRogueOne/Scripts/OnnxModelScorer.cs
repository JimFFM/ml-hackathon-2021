using System.Collections.Generic;
using System.Linq;

using Microsoft.ML;
using Microsoft.ML.Data;


namespace GodotGym
{
    public class OnnxModelScorer
    {
        private readonly string modelLocation;
        private readonly MLContext mlContext;
        private ITransformer model;

        public OnnxModelScorer(string modelLocation, MLContext mlContext)
        {
            this.modelLocation = modelLocation;
            this.mlContext = mlContext;
        }

        public class ModelInput
        {
            [VectorType(10)]
            [ColumnName("input.1")]
            public float[] Features { get; set; }
            public ModelInput(IEnumerable<float> input)
            {
                Features = input.ToArray();
            }
            public static List<ModelInput> MakeInput(IEnumerable<float> input)
            {
                List<ModelInput> input_array = new List<ModelInput>();
                input_array.Add(new ModelInput(input));
                return input_array;
            }

        }

        private ITransformer LoadModel(string modelLocation)
        {

            // Create IDataView from empty list to obtain input data schema
            var data = mlContext.Data.LoadFromEnumerable(new List<ModelInput>());

            // Define scoring pipeline
            var pipeline = mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { "31", "34" }, inputColumnNames: new[] { "input.1" });

            // Fit scoring pipeline
            var model = pipeline.Fit(data);

            return model;
        }
        public class Prediction
        {
            [VectorType(6)]
            [ColumnName("31")]
            public float[] action { get; set; }
            [VectorType(1)]
            [ColumnName("34")]
            public float[] state { get; set; }
        }
        private IEnumerable<float> PredictDataUsingModel(IDataView testData, ITransformer model)
        {

            IDataView scoredData = model.Transform(testData);

            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>("31");
            var a = probabilities.ToList();
            a.Count.ToString();
            return a[0];
        }

        public IEnumerable<float> Score(ModelInput input)
        {
            var data = mlContext.Data.LoadFromEnumerable(new[] { input });
            if (model == null)
                model = LoadModel(modelLocation);

            return PredictDataUsingModel(data, model);
        }
        public IEnumerable<float> Score(IEnumerable<float> input)
        {
            var data = mlContext.Data.LoadFromEnumerable(new[] { new ModelInput(input) });
            if (model == null)
                model = LoadModel(modelLocation);

            return PredictDataUsingModel(data, model);
        }
    }
}

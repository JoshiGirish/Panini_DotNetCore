using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFIDF
{
    class MLPipeline
    {
        public PredictionEngine<TextData, TransformedTextData> mlengine;

        public MLPipeline()
        {
            // Initialize ML pipeline
            var mlcontext = new MLContext();
            var emptyData = new List<TextData>();
            var emptyDataView = mlcontext.Data.LoadFromEnumerable(emptyData);
            var textPipeline = mlcontext.Transforms.Text.TokenizeIntoWords("Words", "Text", separators: new[] { ' ', '\n', ',', '.', '!', '(', ')', '\t', ':', ';' })
                .Append(mlcontext.Transforms.Text.RemoveDefaultStopWords("Words", "Words", Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));
            var textTransformer = textPipeline.Fit(emptyDataView);
            mlengine = mlcontext.Model.CreatePredictionEngine<TextData, TransformedTextData>(textTransformer);
        }
    }

    public class TextData
    {
        public string Text { get; set;}
    }

    public class TransformedTextData
    {
        public string[] Words { get; set; }
    }
}

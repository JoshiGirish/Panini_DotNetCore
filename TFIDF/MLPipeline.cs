using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFIDF
{
    /// <summary>
    /// The class that creates the machine learning pipeline.
    /// </summary>
    public class MLPipeline
    {
        /// <summary>
        /// Prediction engine for making predictions.
        /// </summary>
        /// <value>The ML engine used for predictions using previously trained model.</value>
        public PredictionEngine<TextData, TransformedTextData> mlengine;

        #region Constructor
        /// <summary>
        /// Creates a machine learning engine for transforming text data. 
        /// The engine takes text data, tokenizes it into words, and removes the stop words.
        /// </summary>
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
        #endregion
    }
}

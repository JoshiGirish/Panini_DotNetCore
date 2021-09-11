namespace TFIDF
{
    /// <summary>
    /// The class that represents the data being output by the ML engine.
    /// </summary>
    public class TransformedTextData
    {
        /// <summary>
        /// Transformed text data.
        /// </summary>
        /// <value>An array of predicted words created at the end of the text transformation pipeline.</value>
        public string[] Words { get; set; }
    }
}

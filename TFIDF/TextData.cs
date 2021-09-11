namespace TFIDF
{
    /// <summary>
    /// The class that represents the text data to be provided to the ML engine.
    /// </summary>
    public class TextData
    {
        /// <summary>
        /// Text string to be passed for creating the ML engine.
        /// </summary>
        /// <value>Text used to fit train the text transformer of the engine.</value>
        public string Text { get; set;}
    }
}

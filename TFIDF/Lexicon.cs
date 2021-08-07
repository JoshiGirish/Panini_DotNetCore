using System.Collections.Generic;
using System.Linq;

namespace TFIDF
{
    public class Lexicon
    {
        public static IEnumerable<string> words = new List<string>();
        public static int vocabsize { get { return words.Count(); } set { } }

        public IEnumerable<string> update_from(List<string> tokens)
        {
            words = words.Union(tokens);
            return words;
        }
    }
}

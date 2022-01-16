using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TFIDF
{
    /// <summary>
    /// The class that represents the lexicon/vocabulary of a given corpus of topics.
    /// </summary>
    public static class Lexicon
    {
        /// <summary>
        /// An <c>IEnumerable</c> of words of the lexicon.
        /// </summary>
        /// <value>The words that compose the lexicon.</value>
        public static IEnumerable<string> words = new List<string>();

        /// <summary>
        /// Maximum vocabulary size.
        /// </summary>
        /// <value>The maximum number of words allowed to be part of the lexicon.</value>
        public static int MaxVocabSize = 500;

        //public static int vocabsize { get { return words.Count(); } set { } }

        /// <summary>
        /// A dictionary for storing word counts.
        /// </summary>
        /// <value>A thread-safe dictionary for storing counts of the words in the lexicon.</value>
        public static ConcurrentDictionary<string, int> wordsDictionary = new ConcurrentDictionary<string, int>();

        #region Update word counts
        /// <summary>
        /// Updates the counts of words of the lexicon from a give list of tokens (<paramref name="tokens"/>).
        /// </summary>
        /// <param name="tokens">A list of tokens.</param>
        public static void update_from(List<string> tokens)
        {
            var tokenGroups = tokens.GroupBy(n => n);
            var initial_count = 0;
            foreach(var group in tokenGroups)
            {
                if(wordsDictionary.TryGetValue(group.Key,out initial_count))
                {
                    wordsDictionary[group.Key] = initial_count + group.Count();
                }
                wordsDictionary.TryAdd(group.Key, group.Count());
            }
            //words = words.Union(tokens);
            //return words;
        }
        #endregion

        #region Update words of lexicon
        /// <summary>
        /// Updates the words of the lexicon from the word-count dictionary (<see cref="wordsDictionary"/>).
        /// </summary>
        public static void update_lexicon_words()
        {
                words = wordsDictionary.OrderByDescending(n => n.Value).Take(MaxVocabSize).Select(n => n.Key);
        }
        #endregion

        /// <summary>
        /// Resets the lexicon.
        /// </summary>
        /// <value>Resets the lexicon words to an empty list.</value>
        public static void reset_lexicon()
        {
            words = new List<string>();
            wordsDictionary = new ConcurrentDictionary<string, int>();
        }
        //static Lexicon(int maxSize)
        //{
        //    MaxVocabSize = maxSize;
        //}

    }
}

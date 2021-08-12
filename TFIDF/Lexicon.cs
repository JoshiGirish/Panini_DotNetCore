using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TFIDF
{
    public static class Lexicon
    {
        public static IEnumerable<string> words = new List<string>();
        public static int MaxVocabSize = 500;
        public static int vocabsize { get { return words.Count(); } set { } }
        public static ConcurrentDictionary<string, int> wordsDictionary = new ConcurrentDictionary<string, int>();
        public static object lockKey;

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

        public static void update_lexicon_words()
        {
                words = wordsDictionary.OrderByDescending(n => n.Value).Take(MaxVocabSize).Select(n => n.Key);
        }

        //static Lexicon(int maxSize)
        //{
        //    MaxVocabSize = maxSize;
        //}

    }
}

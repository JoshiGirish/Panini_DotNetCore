using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TFIDF
{
    class Program
    {

        public static Corpus corpus { get; set; }

        public static void Main(string[] args)
        {
            //var directory = @"C:\Users\Girish\Downloads\python-3.9.6-docs-html\library";
            var directory = @"C:\Users\Girish\Downloads\wiki";
            corpus = new Corpus(directory);
            foreach(var topic in corpus.topics)
            {
                List<string> relTopics = new List<string>();
                var similarTopics = corpus.topics.OrderByDescending(n => n.tfidf.similarityScores[topic.topicName]).Take(5);
                
                Console.WriteLine($"\n\n ----------- Topics similar to : {topic.topicName} ----------------");
                foreach(var top in similarTopics)
                {
                    var similarWords = Lexicon.words.OrderByDescending(word => top.tfidf.tfidfVector[word] * topic.tfidf.tfidfVector[word]).Take(6);
                    var words = similarWords.ToList();
                    Console.WriteLine(top.topicName + $".html \t Score: {Math.Round(top.tfidf.similarityScores[topic.topicName],2)} \t Words: {string.Join(",", words)}");

                }
            }
            Console.ReadLine();
        }
    }
}

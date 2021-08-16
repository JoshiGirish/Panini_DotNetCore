using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;
using System.Threading;
using HtmlAgilityPack;

namespace TFIDF
{
    public class Corpus : BaseModel
    {

        #region Property Declarations
        public static ConcurrentQueue<Topic> concqueue = new ConcurrentQueue<Topic>();
        public List<Topic> topics = new List<Topic>();
        public static List<string> topicNames = new List<string>();
        public static Dictionary<string, Topic> topicMap = new Dictionary<string, Topic>();
        //public static Lexicon vocabulary;
        public ConcurrentDictionary<string, Topic> concDict;
        public static string targetDir;
        public static DirectoryInfo targetDirInfo;
        public static FileInfo[] Files;
        private List<string> validFileExtensions = new List<string>() { ".html", ".htm" };
        public Stopwatch stopwatch;

        public List<string> ignoreTopicNameEndsWith = new List<string> { "-r-wn", "-sb", "-c-ov", "-c-AccessToContent", "-r-rg", "-r-ui-ContextMenus", "-ActBar" };
        public List<string> ignoreTopicNameContains = new List<string> { "-ActBar", "-ContexMenus-" };
        public List<string> ignoreTopicNameStartsWith = new List<string> { "help-rc" };
        public List<string> ignoreTopicName = new List<string> { "Sitemap" };
        public Dictionary<string, List<string>> ignoreData;
        public int wordsMax;
        public int xrefsMax;
        public int relinksMax;


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the corpus data.
        /// </summary>
        /// <param name="directory"></param>
        public Corpus(string directory, int maxSize)
        {
            //read_files(directory);
            targetDir = directory;
            initializeDict();

            // Initialize lexicon
            Lexicon.MaxVocabSize = maxSize;

            ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };


            // Execute stages asynchronously on multiple threads
            //generate_topics_async();
            //calculate_tfidf_scores_async();
            //calculate_topic_similarity_scores_async();

            // Execute stages synchronously on main thread
            //generate_topics();
            //calculate_tfidf_scores();
            //calculate_topic_similarity_scores();
        }
        #endregion

        #region Reset Corpus
        /// <summary>
        /// Resets the corpus data.
        /// </summary>
        public void reset_corpus(int maxSize)
        {
            concqueue = new ConcurrentQueue<Topic>{ };
            topics = new List<Topic>();
            topicNames = new List<string>();
            topicMap = new Dictionary<string, Topic>();
            Lexicon.MaxVocabSize = maxSize;
            concDict = new ConcurrentDictionary<string, Topic>();
        }
        #endregion

        #region Read Files
        /// <summary>
        /// Returns the files from the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public IEnumerable<string> read_files(string directory)
        {
            // Get all the files in the target directory
            //targetDir = directory;
            targetDirInfo = new DirectoryInfo(directory);
            Files = targetDirInfo.GetFiles("*.htm*");
            return Files.Select(n => n.Name);
        }
        #endregion

        #region Initialize Concurrent Dictionary
        /// <summary>
        /// Initializes the concurrent dictionary used to store the computed topic data.
        /// Note: Concurrent dictionary allows thread-safe access to the class instance resources when multi-threading.
        /// </summary>
        public void initializeDict()
        {
            // 
            int NUMITEMS = topics.Count;
            int initialCapacity = NUMITEMS * 2;

            int numProcessors = System.Environment.ProcessorCount;
            int concurrencyLevel = numProcessors * 2;
            concDict = new ConcurrentDictionary<string, Topic>(concurrencyLevel, initialCapacity);
        }
        #endregion

        #region Generate Topics
        /// <summary>
        /// Generate the topic instances from the files in the directory.
        /// </summary>
        public void generate_topics()
        {
            stopwatch = Stopwatch.StartNew();
            ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };
            foreach (var file in Files)
            {
                if (Topic.Is_valid(file.FullName, ignoreData))
                {
                    var topic = instantiate_topic(file, targetDir, ignoreData);
                    topics.Add(topic);
                    update_lexicon(topic);
                }
            }
            var instantiationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" Topic instantiation ---> {instantiationTime} sec"); // 5 sec
        }
        #endregion

        #region Calculate TFIDF Scores
        /// <summary>
        /// Calculates the TFIDF scores for each topic.
        /// </summary>
        public void calculate_tfidf_scores()
        {
            stopwatch = Stopwatch.StartNew();
            foreach (var topic in topics)
            {
                topic.tfidf = new TFIDF(topic, topics);
            }
            var tfidfCalculationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" TFIDF Calculation   ---> {tfidfCalculationTime} sec"); // 55 sec
        }
        #endregion

        #region Calculate Topic Similarity Scores
        /// <summary>
        /// Calculates the similarity scores for each topic and stores it in the "similarityScores" list of the topic instance.
        /// </summary>
        public void calculate_topic_similarity_scores()
        {
            stopwatch = Stopwatch.StartNew();
            foreach (var sourceTopic in topics)
            {
                foreach (var corpusTopic in topics)
                {
                    var score = cosine_similarity(sourceTopic, corpusTopic);
                    sourceTopic.tfidf.similarityScores.Add(corpusTopic.topicName, score);
                }
            }
            stopwatch.Stop();
            var similarityScoreCalculationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" Score Calculation   ---> {similarityScoreCalculationTime} sec"); // 12 sec
        }
        #endregion

        #region Generate Topics (Multi-threading)
        /// <summary>
        /// Generates topic class instances from files in the directory using multi-threading.
        /// </summary>
        public void generate_topics_async()
        {
            ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };

            stopwatch = Stopwatch.StartNew();

            ParallelLoopResult readTopicResult = Parallel.ForEach(Files,
                                                   (file) =>
                                                   {
                                                       if (Topic.Is_valid(file.FullName, ignoreData))
                                                       {
                                                           var topic = instantiate_topic(file, targetDir, ignoreData);
                                                           if(topic.words != null)
                                                           {
                                                               concqueue.Enqueue(topic);
                                                               update_lexicon(topic);
                                                           }
                                                       }
                                                   });


            while (!concqueue.IsEmpty)
            {
                if (concqueue.TryDequeue(out Topic top))
                {
                    topics.Add(top);
                }
            }
            Lexicon.update_lexicon_words();
            stopwatch.Stop();
            var instantiationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" Topic instantiation ---> {instantiationTime} sec"); // 5 sec
        }
        #endregion

        #region Calculate TFIDF Scores (Multi-threading)
        /// <summary>
        /// Calculates the TFIDF scores for each topic using multi-threading.
        /// </summary>
        public void calculate_tfidf_scores_async()
        {
            stopwatch = Stopwatch.StartNew();
            ParallelLoopResult tfidfCompResult = Parallel.ForEach(topics,
                                                   (topic) => {
                                                       topic.tfidf = new TFIDF(topic, topics);
                                                       concDict.TryAdd(topic.topicName, topic);
                                                   });
            stopwatch.Stop();
            var tfidfCalculationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" TFIDF Calculation   ---> {tfidfCalculationTime} sec"); // 55 sec
        }
        #endregion

        #region Calculate Topic Similarity Scores (Multi-threading)
        /// <summary>
        /// Calculates the similarity scores for each topic using multi-threading.
        /// </summary>
        public void calculate_topic_similarity_scores_async()
        {
            stopwatch = Stopwatch.StartNew();
            foreach (var sourceTopic in concDict.Values)
            {
                foreach (var corpusTopic in concDict.Values)
                {
                    var score = cosine_similarity(sourceTopic, corpusTopic);
                    sourceTopic.tfidf.similarityScores.Add(corpusTopic.topicName, score);
                }
            }
            stopwatch.Stop();
            var similarityScoreCalculationTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine($" Score Calculation   ---> {similarityScoreCalculationTime} sec"); // 12 sec
        }
        #endregion

        #region Instantiate Topic
        /// <summary>
        /// Instantiates the topic class for each file in the directory ignoring files which do not comply with the data stored in "ignoreData" dictionary.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directory"></param>
        /// <param name="ignoreData"></param>
        /// <returns></returns>
        public Topic instantiate_topic(FileInfo file, string directory, Dictionary<string,List<string>> ignoreData)
        {
            // Initialize Topic instance for each file
            var topicName = Path.GetFileNameWithoutExtension(file.FullName);
            var fileName = Path.GetFileName(file.FullName);
            var path = Path.Combine(directory, topicName + file.Extension);
            Console.WriteLine("\n\n ----------- " + topicName + "----------------\n");
            var doc = new HtmlDocument();
            doc.Load(path);
            Topic topic = new Topic(fileName, path, doc, ignoreData);
            if(topic.words != null) topicNames.Add(topicName);

            return topic;
        }
        #endregion

        #region Update Lexicon
        /// <summary>
        /// Appends words in the lexicon with new words from the given topic.
        /// The size of the lexicons grows as we call this method on each topic instantiation.
        /// </summary>
        /// <param name="topic"></param>
        public void update_lexicon(Topic topic)
        {
            // Update the lexicon
            Console.WriteLine(topic.words.Count);
            Lexicon.update_from(topic.words);
            Console.WriteLine(Lexicon.words.Count());
        }
        #endregion

        #region Cosine Similarity
        /// <summary>
        /// Computes the cosine similarity between the TFIDF vectors of two given topics.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double cosine_similarity(Topic source, Topic target)
        {
            double num = 0;
            double x = 0;
            double y = 0;
            double den = 0;

            foreach (var word in Lexicon.words)
            {
                double a = source.tfidf.tfidfVector[word];
                double b = target.tfidf.tfidfVector[word];
                num += a * b;
                x += a * a;
                y += b * b;
            }
            den = Math.Sqrt(x) * Math.Sqrt(y);
            return num / den;
        }
        #endregion

        #region Get Similar Topics
        /// <summary>
        /// Returns an IEnumerable of similar topic instances for a given topic.
        /// The size of the IEnumerable is equal to the requested number of similar topics.
        /// </summary>
        /// <param name="topic"> The topic for which other similar topics are to be fetched.</param>
        /// <param name="numOfTopics"> The number of similar topics to be fetched.</param>
        /// <returns></returns>
        public IEnumerable<Topic> get_similar_topics(Topic topic, int numOfTopics)
        {
            return topics.OrderByDescending(n => n.tfidf.similarityScores[topic.topicName]).Take(numOfTopics+1).Skip(1); 
            // numOfTopics + 1 -> as the most similar topic is the topic itself, which we skip
        }
        #endregion

        #region Get Similarity Score
        /// <summary>
        /// Returns a similarity score (between 0 and 1) between two given topics.
        /// 0 -> No similarity
        /// 1 -> Complete similarity
        /// </summary>
        /// <param name="sourceTopic">The source topic.</param>
        /// <param name="targetTopic">The target topic.</param>
        /// <returns></returns>
        public double get_similarity_score(Topic sourceTopic, Topic targetTopic)
        {
            return Math.Round(targetTopic.tfidf.similarityScores[sourceTopic.topicName]*100, 1);
        }
        #endregion

        #region Get Similar Words
        /// <summary>
        /// Returns a list of commnon words between two given topics.
        /// </summary>
        /// <param name="sourceTopic">The source topic.</param>
        /// <param name="targetTopic">The target topic.</param>
        /// <param name="numOfWords">Number of most common words.</param>
        /// <returns></returns>
        public List<string> get_similar_words(Topic sourceTopic, Topic targetTopic, int numOfWords)
            {
                var similarWords = Lexicon.words.OrderByDescending(word => targetTopic.tfidf.tfidfVector[word] * sourceTopic.tfidf.tfidfVector[word]).Take(numOfWords>10?10:numOfWords);
                return similarWords.ToList();
            }
        #endregion

        #region Compute Max
        public void compute_max()
        {
            // Compute the maximum words, xrefs, and related links from all the topics
            wordsMax = topics.Select(topic => topic.words.Count).ToArray().Max();
            xrefsMax = topics.Select(topic => topic.xrefs.Count).ToArray().Max();
            relinksMax = topics.Select(topic => topic.relinks.Count).ToArray().Max();
        }
        #endregion
    }
}

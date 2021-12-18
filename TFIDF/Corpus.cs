﻿using System;
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
    /// <summary>
    /// The class that represents the corpus of the web-topics being processed.
    /// </summary>
    public class Corpus : BaseModel
    {

        #region Property Declarations

        /// <summary>
        /// A thread-safe FIFO collection of <c>Topic</c> instances.
        /// </summary>
        /// <value>Offers a thread-safe queue for storing the <c>Topic</c> instances generated by multiple threads.</value>
        public static ConcurrentQueue<Topic> concqueue = new ConcurrentQueue<Topic>();

        /// <summary>
        /// A collection of <c>Topic</c> instances.
        /// </summary>
        /// <value>Stores the <c>Topic</c> instances, when they are not generated using multiple threads.</value>
        public List<Topic> topics = new List<Topic>();

        /// <summary>
        /// A list of web-topic names.
        /// </summary>
        /// <value>Stores the names of the web-topics (without extension).</value>
        public static List<string> topicNames = new List<string>();

        /// <summary>
        /// A dictionary with topic name as key and <c>Topic</c> instance as value.
        /// </summary>
        /// <value>Represents a mapping of topic names with <c>Topic</c> instances.</value>
        public static Dictionary<string, Topic> topicMap = new Dictionary<string, Topic>();
        
        /// <summary>
        /// A thread-safe dicitionary for storing <c>Topic</c> instances.
        /// </summary>
        /// <value>Offers a thread safe dictionary for storing topics, when topics are generated using multiple threads.</value>
        public ConcurrentDictionary<string, Topic> concDict;

        /// <summary>
        /// The path from which the web-topics are read.
        /// </summary>
        /// <value>Represents the location of the directory in which the source web-topics are stored. 
        /// <para>Note: The web-topics must reside in this directory and not in sub-directories.</para></value>
        public static string targetDir;

        /// <summary>
        /// A <c>DirectoryInfo</c> instance for <c>targetDir</c>.
        /// </summary>
        /// <value>Used for reading web-topics from <c>targetDir</c>.</value>
        public static DirectoryInfo targetDirInfo;

        /// <summary>
        /// An array of <c>FileInfo</c> instances for each web-topic being read.
        /// </summary>
        /// <value>Used to iterate over files when creating <c>Topic</c> instances.</value>
        public static FileInfo[] Files;

        /// <summary>
        /// A list of valid extensions.
        /// </summary>
        /// <value>Represents a list of valid extensions for the web-topics being read.</value>
        private List<string> validFileExtensions = new List<string>() { ".html", ".htm" };

        /// <summary>
        /// A <c>Stopwatch</c> instance.
        /// </summary>
        /// <value>Used for measuring time required for generating <c>Topic</c> instances, TFIDF computation, and similarity score computation.</value>
        public Stopwatch stopwatch;

        /// <summary>
        /// A list of ending strings to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the end of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        public List<string> ignoreTopicNameEndsWith = new List<string> { "-r-wn", "-sb", "-c-ov", "-c-AccessToContent", "-r-rg", "-r-ui-ContextMenus", "-ActBar" };

        /// <summary>
        /// A list of strings to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched anywhere in the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        public List<string> ignoreTopicNameContains = new List<string> { "-ActBar", "-ContexMenus-" };

        /// <summary>
        /// A list of starting strings to ivalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the beginning of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        public List<string> ignoreTopicNameStartsWith = new List<string> { "help-rc" };

        /// <summary>
        /// A list of topic names used to invalidate web-topics.
        /// </summary>
        /// <value>Represents a set of substrings which when matched at the end of the web-topic names, results in the topic being ignored from the TFIDF analysis.</value>
        public List<string> ignoreTopicName = new List<string> { "Sitemap" };

        /// <summary>
        /// A dictionary for gathering the topic invalidation data.
        /// </summary>
        /// <value>Represents a container for storing objects used for invalidating web-topics.
        /// <para>This dictionary is used for passing the ignore data easily to other methods. This dictionary gathers the following fields:
        /// <list type="bullet">
        ///     <item><term><see cref="ignoreTopicNameEndsWith"/></term></item>
        ///     <item><term><see cref="ignoreTopicNameStartsWith"/></term></item>
        ///     <item><term><see cref="ignoreTopicNameContains"/></term></item>
        ///     <item><term><see cref="ignoreTopicName"/></term></item>
        /// </list>
        /// </para></value>
        public Dictionary<string, List<string>> ignoreData;

        /// <summary>
        /// The HTML tag of the topic title.
        /// </summary>
        public string titleTag;

        /// <summary>
        /// The max word count.
        /// </summary>
        /// <value>Represents the number of words of the topic with the highest word count.</value>
        public int wordsMax;

        /// <summary>
        /// The max inline link count.
        /// </summary>
        /// <value>Represents the number of inline links from the topic with the highest inline link count.</value>
        public int xrefsMax;

        /// <summary>
        /// The max related link count.
        /// </summary>
        /// <value>Rpresents the number of related links from the topic with the highest related link count.</value>
        public int relinksMax;

        /// <summary>
        /// The selection mode of the related links container.
        /// </summary>
        /// <value>Represents the mode of selection, for extracting the parent container tag which includes are the anchor tags of the related links.</value>
        public string Mode = "innerText";

        /// <summary>
        /// The comma separated inner texts of the related links containers.
        /// </summary>
        /// <value>Represents a comma separated string of the inner texts of the parent container of the related links.
        /// <para>Usually web-topics contain related links under a section named <c>See Also</c> or <c>Related Topics</c>. 
        /// These section headings are passed using this <c>InnerText</c> field.</para></value>
        public string InnerText = "";

        /// <summary>
        /// The CSS selector of the parent of the related links.
        /// </summary>
        /// <value>Represents the CSS Selector of the related link tag.</value>
        public string CSSSelector = "";

        /// <summary>
        /// The ancestor level of the selected tag.
        /// </summary>
        /// <value>Sometimes the tag of the section headings for the related links is not their parent tag, but siblings. 
        /// In such cases, the <c>AncestorLevel</c> field lets you specify which level of parent must be selected that will encapsulate all related links.</value>
        public int AncestorLevel = 0;


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the corpus data.
        /// </summary>
        /// <param name="directory">The path to the directory (<see cref="targetDir"/>) that contains the web-topics.</param>
        /// <param name="maxSize">The max vocabulary size of the lexicon. It represents the maximum number of allowed words in the lexicon.</param>
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
        /// <para>The <c>Corpus</c> must be rebuilt everytime the user loads new web-topics.</para>
        /// </summary>
        /// <param name="maxSize">The max vocabulary size of the lexicon. It represents the maximum number of allowed words in the lexicon.</param>
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
        /// Returns the files from the given <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The directory from which the files need to be read.</param>
        /// <returns>An <c>IEnumerable</c> of the names of the files in the directory.</returns>
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
        /// Note: Concurrent dictionary allows thread-safe access to the class instance resources for multi-threading.
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
        /// Generates the <c>Topic</c> instances from the files in the directory.
        /// </summary>
        public void generate_topics()
        {
            stopwatch = Stopwatch.StartNew();
            ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };
            var selectionData = new Dictionary<string, string>() { {"Mode", Mode },
                                                               { "InnerText", InnerText},
                                                                {"CSSSelector", CSSSelector }};

            foreach (var file in Files)
            {
                if (Topic.Is_valid(file.FullName, ignoreData))
                {
                    var topic = instantiate_topic(file, targetDir, ignoreData, selectionData, AncestorLevel, titleTag);
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
        /// Calculates the similarity scores for each topic and stores it in the <c>similarityScores</c> list of the <c>Topic</c> instance.
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
        /// Generates <c>Topic</c> instances from files in the directory using multi-threading.
        /// </summary>
        public void generate_topics_async()
        {
            ignoreData = new Dictionary<string, List<string>>(){{"end",  ignoreTopicNameEndsWith},
                                                                {"start", ignoreTopicNameStartsWith},
                                                                {"contains", ignoreTopicNameContains},
                                                                {"name", ignoreTopicName } };

            var selectionData = new Dictionary<string, string>() { {"Mode", Mode },
                                                               { "InnerText", InnerText},
                                                                {"CSSSelector", CSSSelector }, };

            stopwatch = Stopwatch.StartNew();

            ParallelLoopResult readTopicResult = Parallel.ForEach(Files,
                                                   (file) =>
                                                   {
                                                       if (Topic.Is_valid(file.FullName, ignoreData))
                                                       {

                                                           var topic = instantiate_topic(file, targetDir, ignoreData, selectionData, AncestorLevel, titleTag);
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

        #region Calculate TFIDF Scores (Parallel Processing)
        /// <summary>
        /// Calculates the TFIDF scores of each topic using parallel computing.
        /// </summary>
        public void calculate_tfidf_scores_parallel()
        {
            var nRows = concDict.Values.Count;
            var nCols = Lexicon.words.Count();
            var wordCountMatrix = new int[nRows, nCols];
            var tfMatrix = new double[nRows, nCols];

            // Generate matrix
            for (var i = 0; i < nRows; i++)
            {
                var topic = concDict.Values.ToArray()[i];
                var wordCounts = topic.tfidf.wordCountVector.Values.ToArray();
                var maxCount = wordCounts.Max();
                for (var j = 0; j < nCols; j++)
                {
                    wordCountMatrix[i, j] = wordCounts[j];
                    tfMatrix[i, j] = wordCounts[j] / maxCount;
                }
            }
        }
        #endregion

        #region Instantiate Topic
        /// <summary>
        /// Instantiates the <c>Topic</c> class for each file in the directory ignoring files which do not comply with the data stored in <see cref="ignoreData"/>.
        /// </summary>
        /// <param name="file">The file against which the <c>Topic</c> instance is to be generated.</param>
        /// <param name="directory">The path of directory of the file.</param>
        /// <param name="ignoreData">Contains data to verify if the filename complies with configured options.</param>
        /// <param name="selectionData">Contains data used for extracting related links from the file.</param>
        /// <returns></returns>
        public Topic instantiate_topic(FileInfo file, string directory, Dictionary<string,List<string>> ignoreData, Dictionary<string,string> selectionData, int level, string titleTag)
        {
            // Initialize Topic instance for each file
            var topicName = Path.GetFileNameWithoutExtension(file.FullName);
            var fileName = Path.GetFileName(file.FullName);
            var path = Path.Combine(directory, topicName + file.Extension);
            Console.WriteLine("\n\n ----------- " + topicName + "----------------\n");
            var doc = new HtmlDocument();
            doc.Load(path);
            Topic topic = new Topic(fileName, path, doc, ignoreData, selectionData, level, titleTag);
            if(topic.words != null) topicNames.Add(topicName);

            return topic;
        }
        #endregion

        #region Update Lexicon
        /// <summary>
        /// Appends new words from the given topic to the <see cref="Lexicon"/>.
        /// The size of the lexicon grows as we call this method on each topic instantiation.
        /// </summary>
        /// <param name="topic">The topic whose words are being considered for updating the <see cref="Lexicon"/>.</param>
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
        /// <param name="source">The source topic.</param>
        /// <param name="target">The target topic.</param>
        /// <returns>The cosine similarity score between the topics.</returns>
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
        /// Returns an IEnumerable of similar <c>Topic</c> instances for a given topic (<paramref name="topic"/>).
        /// The size of the IEnumerable is equal to the requested number (<paramref name="numOfTopics"/>) of similar topics.
        /// </summary>
        /// <param name="topic"> The topic for which other similar topics are to be found.</param>
        /// <param name="numOfTopics"> The number of similar topics to be found.</param>
        /// <returns></returns>
        public IEnumerable<Topic> get_similar_topics(Topic topic, int numOfTopics)
        {
            return topics.OrderByDescending(n => n.tfidf.similarityScores[topic.topicName]).Take(numOfTopics+1).Skip(1); 
            // numOfTopics + 1 -> as the most similar topic is the topic itself, which we skip
        }
        #endregion

        #region Get Similarity Score
        /// <summary>
        /// Returns a similarity score (0 to 1) between two given topics.
        /// <para>0 -> No similarity</para>
        /// <para>1 -> Complete similarity</para>
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
        /// Finds a list of commnon words between the two given topics.
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
        /// <summary>
        /// Computes the highest number of words, inline links, and related links found in a topic.
        /// </summary>
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

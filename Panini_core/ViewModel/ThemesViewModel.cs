using Panini.Commands;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Aglomera;
using ExamplesUtil;
using System.Linq;
using Aglomera.Linkage;
using System.Text.Json;
using System.IO;
using Kneedle;
using LiveCharts.Defaults;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace Panini.ViewModel
{
    public class ThemesViewModel : BaseViewModel
    {

        #region Property Declarations

        /// <summary>
        /// Single instance of <see cref="DataCache"/>.
        /// </summary>
        /// <value>Used for sharing resources between view models.</value>
        private readonly DataCache dataCache = DataCache.Instance;

        private string _rank;

        public string Rank
        {
            get { return _rank; }
            set { _rank = value; RaisePropertyChanged(); }
        }

        private string _matrix;

        public string Matrix
        {
            get { return _matrix; }
            set { _matrix = value; RaisePropertyChanged(); }
        }

        private ICommand _run;

        public ICommand Run
        {
            get { return _run ?? (_run = new ButtonCommandHandler(() => RunCallback(), () => true)); }
        }

        private ICommand _updateTree;
        public ICommand UpdateTree
        {
            get {
                return _updateTree ?? (_updateTree = new ChartCommandHandler<ChartPoint>
                {
                    ExecuteDelegate = p => update_tree(p)
                }); }
        }

        private ObservableCollection<Item> _dataSource = new ObservableCollection<Item>();

        public ObservableCollection<Item> DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<string> _linkages;
        public ObservableCollection<string> Linkages
        {
            get { return _linkages; }
            set { _linkages = value; RaisePropertyChanged(); }
        }

        private string _selectedLinkage = "MIN-ENERGY";
        public string SelectedLinkage
        {
            get { return _selectedLinkage; }
            set { _selectedLinkage = value; RaisePropertyChanged(); update_chart(); }
        }

        private string _themesViewVisibility = "Collapsed";
        public string ThemesViewVisibility
        {
            get { return _themesViewVisibility; }
            set { _themesViewVisibility = value; RaisePropertyChanged(); }
        }

        private double[] _chartXSeries = new double[]{};

        private double[] _chartYSeries = new double[] { };

        private double[] _chartYSeriesFullPrecision = new double[] { };



        private Dictionary<string, Dictionary<string, int?>> _knees = new Dictionary<string, Dictionary<string, int?>>() { };

        private SortedDictionary<string, string> LinkageMap = new SortedDictionary<string, string>()
        {
            { "AVERAGE" ,"average"  },
            {  "SINGLE", "single" },
            { "COMPLETE", "complete" },
            {   "MIN-ENERGY","min-energy"},
            { "CENTROID","centroid" },
            { "WARD", "ward" }
        };

        private Dictionary<string, ClusteringResult<DataPoint>> clusterings = new Dictionary<string, ClusteringResult<DataPoint>>() { };

        private int _distanceSliderValue = 0;
        public int DistanceSliderValue
        {
            get { return _distanceSliderValue; }
            set { _distanceSliderValue = value; SubClusterDistance = _chartYSeries[value]; sub_cluster(); RaisePropertyChanged(); }
        }

        private int _maxValue = 1;
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; RaisePropertyChanged(); }
        }


        private double _selectedDissimilarity = 0.0;
        public double SelectedDissimilarity
        {
            get { return _selectedDissimilarity; }
            set { _selectedDissimilarity = value; RaisePropertyChanged(); }
        }

        private int _selectedClusteringLevel = 1;
        public int SelectedClusteringLevel
        {
            get { return _selectedClusteringLevel; }
            set { _selectedClusteringLevel = value; RaisePropertyChanged(); }
        }


        private int[] _ticks = { };
        public int[] Ticks
        {
            get { return _ticks; }
            set { _ticks = value; RaisePropertyChanged(); }
        }

        private double _subClusterDistance = 0.0;
        public double SubClusterDistance
        {
            get { return _subClusterDistance; }
            set { _subClusterDistance = value; RaisePropertyChanged(); }
        }

        private double _fuzzyness = 0.0;
        public double FuzzynessSliderValue
        {
            get { return _fuzzyness; }
            set { _fuzzyness = value; sub_cluster(); RaisePropertyChanged(); }
        }

        #region Processing Flag
        /// <summary>
        /// Flag to display the processing prompt.
        /// </summary>
        private string _processingMessageVisibility = "Collapsed";
        /// <summary>
        /// Controls visibility of the processing message.
        /// </summary>
        /// <value>Controls the visibility of the processing message in the themes view when the hierarchical clusterings are being computed.</value>
        public string ProcessingMessageVisibility
        {
            get { return _processingMessageVisibility; }
            set { _processingMessageVisibility = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Processing Stage
        private string _processingStage;
        /// <summary>
        /// Current processing stage message.
        /// </summary>
        /// <value>The message prompt for displaying clustering is in progress. 
        /// </value>
        public string ProcessingStage
        {
            get { return _processingStage; }
            set { _processingStage = value; RaisePropertyChanged(); }
        }

        #endregion

        #endregion


        private HashSet<DataPoint> get_datapoints()
        {
            var doubArray = dataCache.corpus.get_term_doucment_matrix();
            var dataPoints = new HashSet<DataPoint>();
            var topicNames = dataCache.corpus.topics.Select(topic => topic.topicName).ToArray();

            for (var i=0; i < doubArray.GetLength(1); i++)
            {
                var list = new List<double>() { };

                for(var j=0; j<doubArray.GetLength(0); j++)
                {
                    list.Add(doubArray[j, i]);
                }
                dataPoints.Add(new DataPoint(topicNames[i], list.ToArray()));
            }
            return dataPoints;
        }

        private static ClusteringResult<DataPoint> get_clustering(ISet<DataPoint> instances, ILinkageCriterion<DataPoint> linkage, string name)
        {
            var clusteringAlg = new AgglomerativeClusteringAlgorithm<DataPoint>(linkage);
            var clustering = clusteringAlg.GetClustering(instances);

            Console.WriteLine("_____________________________________________");
            Console.WriteLine(name);    
            foreach (var clusterSet in clustering)
            {
                Console.WriteLine($"Clusters at distance: {clusterSet.Dissimilarity:0.00} ({clusterSet.Count})");
                foreach (var cluster in clusterSet)
                    Console.WriteLine($" - {cluster}");
            }

            return clustering;
        }

        private void sub_cluster()
        {
            Thread subclusterThread = new Thread(new ThreadStart(create_subclusters));
            subclusterThread.IsBackground = true;
            subclusterThread.Start();
        }

        public void create_clusters()
        {
            // Set the processing message prompt
            ProcessingStage = "Computing Clusters";
            ProcessingMessageVisibility = "Visible";

            var metric = new DataPoint(null, null);
            var instances = get_datapoints();
            var linkages = new Dictionary<ILinkageCriterion<DataPoint>, string>
                           {
                               {new AverageLinkage<DataPoint>(metric), "average"},
                               {new CompleteLinkage<DataPoint>(metric), "complete"},
                               {new SingleLinkage<DataPoint>(metric), "single"},
                               {new MinimumEnergyLinkage<DataPoint>(metric), "min-energy"},
                               {new CentroidLinkage<DataPoint>(metric, DataPoint.GetCentroid), "centroid"},
                               {new WardsMinimumVarianceLinkage<DataPoint>(metric, DataPoint.GetCentroid), "ward"}
                           };

            foreach (var linkage in linkages)
                clusterings[linkage.Value] = get_clustering(instances, linkage.Key, linkage.Value);

            compute_all_knees();

            DataSource = get_tree_nodes(_knees[LinkageMap[SelectedLinkage]]["k_cc"]);
            ChartData = new SeriesCollection { };
            get_chart_data(CurrentChartData);

            ProcessingMessageVisibility = "Collapsed";
            ThemesViewVisibility = "Visible";
            SelectedLinkage = "MIN-ENERGY";
        }

        private void compute_all_knees()
        {
            foreach(var clustering in clusterings)
            {
                var doubArray = new List<string>() { };
                var knees = compute_knees(clustering);
                _knees[clustering.Key] = new Dictionary<string, int?>()
                    {
                        { "k_cc", knees[0] },
                        { "k_c" , knees[1] }
                    };
            }
        }

        public ObservableCollection<Item> get_tree_nodes(int? KneePoint)
        {
            var dictArrays = new Dictionary<string, List<string>>();
            var clusterLevelNode = new ObservableCollection<Item>() { };
            foreach (var clustering in clusterings)
            {
                var themeLabel = new List<string>() { };
                    
                if (clustering.Key == LinkageMap[SelectedLinkage]) ///////////
                {
                    compute_series_points(clustering);
                    foreach (var clusterset in clustering.Value)
                    {
                        int kneeValue = KneePoint ?? _chartXSeries.GetLength(0)-1; // Show the single cluster if no knee point found
                        if (Math.Round(clusterset.Dissimilarity, 2, MidpointRounding.ToEven) == _chartYSeries[kneeValue])
                        {
                            themeLabel.Add(clusterset.Dissimilarity.ToString());
                            var clusterCount = 0;
                            foreach (var cluster in clusterset)
                            {
                                var clusterSetNode = new Item() { };
                                //clusterSetNode.Name = clusterCount.ToString() + " " + cluster.Parent1?.ToString() + " " + cluster.Parent2?.ToString();
                                clusterSetNode.Name = clusterCount.ToString();
                                clusterSetNode.Dissimilarity = cluster.Dissimilarity;
                                clusterSetNode.Children = get_cluster_members(cluster);
                                clusterLevelNode.Add(clusterSetNode);
                                clusterCount++;
                            }
                        }
                    }
                }
                // Write some data
                dictArrays[clustering.Key] = themeLabel;
            }
            string json = JsonSerializer.Serialize(dictArrays);
            File.WriteAllText("jsonData.json", json);

            return clusterLevelNode;

        }

        private void create_subclusters()
        {
            var dictArrays = new Dictionary<string, List<string>>();
            var clusterLevelNode = new ObservableCollection<Item>() { };
            foreach (var clustering in clusterings)
            {
                var themeLabel = new List<string>() { };

                if (clustering.Key == LinkageMap[SelectedLinkage]) ///////////
                {
                    compute_series_points(clustering);
                    var clustersetCount = 0;
                    foreach (var clusterset in clustering.Value)
                    {
                        if (SelectedClusteringLevel == clustersetCount)
                        {
                            themeLabel.Add(clusterset.Dissimilarity.ToString());
                            var clusterCount = 0;
                            foreach (var cluster in clusterset)
                            {
                                var clusterSetNode = new Item() { };
                                //clusterSetNode.Name = clusterCount.ToString() + " " + cluster.Parent1?.ToString() + " " + cluster.Parent2?.ToString();
                                clusterSetNode.Name = clusterCount.ToString();
                                clusterSetNode.BackgroundColor = Color.Violet;
                                clusterSetNode.Dissimilarity = cluster.Dissimilarity;
                                clusterSetNode.Children = DistanceSliderValue == SelectedClusteringLevel ? get_cluster_members(cluster) : bifurcateCluster(cluster,cluster);
                                clusterLevelNode.Add(clusterSetNode);
                                clusterCount++;
                            }
                        }
                        clustersetCount++;
                    }
                }
                // Write some data
                dictArrays[clustering.Key] = themeLabel;
            }
            string json = JsonSerializer.Serialize(dictArrays);
            File.WriteAllText("jsonData.json", json);
            DataSource = clusterLevelNode;
        }

        private ObservableCollection<Item> bifurcateCluster(Cluster<DataPoint> cluster, Cluster<DataPoint> parentCluster)
        {
            var clusterChildren = new ObservableCollection<Item>() { };

            // If the two parents are close enough (distance less than fuzzyness value) return all their children as collection
            if (cluster.Parent1 != null && cluster.Parent2 != null) // return the single datapoint for cluster which has no parents
            {
                var linkage = new MinimumEnergyLinkage<DataPoint>(new DataPoint(null, null));
                var fuzzyFraction = FuzzynessSliderValue / 100;
                var fuzzyness = linkage.Calculate(parentCluster.Parent1, parentCluster.Parent2) * fuzzyFraction;
                if (linkage.Calculate(cluster.Parent1, cluster.Parent2) <= fuzzyness)
                {
                    var parent1Children = get_cluster_members(cluster.Parent1);
                    var parent2Children = get_cluster_members(cluster.Parent2);
                    foreach (var child in parent1Children.Concat(parent2Children))
                    {
                        clusterChildren.Add(child);
                    }
                    return clusterChildren;
                }
            }

            if(cluster.Parent1 != null) // for clusters that are not leaf nodes (single datapoint)
            {
                var clusterParentNode = new Item() { };
                clusterParentNode.Name = cluster.Parent1.ToString();
                //if (SubClusterDepth < SelectedClusteringLevel - DistanceSliderValue)
                if(cluster.Dissimilarity >= _chartYSeriesFullPrecision[DistanceSliderValue]) // decompose clusters between the root cluster distance and sub-cluster distance
                {
                    clusterParentNode.Children = bifurcateCluster(cluster.Parent1, parentCluster);
                }
                else  // aggregate multi-level children for cluster dissimilarity less than sub-cluster distance value
                {
                    var collection = new ObservableCollection<Item>() { };
                    foreach (var datapoint in cluster.Parent1)
                    {
                        collection.Add(new Item
                        {
                            Name = datapoint.ID,
                        });
                    }
                    clusterParentNode.Children = collection;
                }
                clusterChildren.Add(clusterParentNode);
            }

            
            if (cluster.Parent2 != null)
            {
                var clusterParentNode = new Item() { };
                clusterParentNode.Name = cluster.Parent2.ToString();
                if (cluster.Dissimilarity >= _chartYSeriesFullPrecision[DistanceSliderValue])
                {
                    clusterParentNode.Children = bifurcateCluster(cluster.Parent2, parentCluster);
                }
                else
                {
                    var collection = new ObservableCollection<Item>() { };
                    foreach(var datapoint in cluster.Parent2)
                    {
                        collection.Add(new Item
                        {
                            Name = datapoint.ID,

                        }) ;
                    }
                    clusterParentNode.Children = collection;
                }
                clusterChildren.Add(clusterParentNode);
            }

            if(cluster.Parent1 == null && cluster.Parent2 == null) // return the single datapoint for cluster which has no parents
            {
                foreach (var datapoint in cluster)
                {
                    clusterChildren.Add(new Item
                    {
                        Name = datapoint.ID
                    });
                }
            }

            return clusterChildren;
        }


        private ObservableCollection<Item> get_cluster_members(Cluster<DataPoint> cluster)
        {
            ObservableCollection<Item> members = new ObservableCollection<Item>() { };
            foreach (var datapoint in cluster)
            {
                var clusterNode = new Item() { };
                clusterNode.Name = datapoint.ID;
                members.Add(clusterNode);
            }
            return members;
        }

        private int?[] compute_knees(KeyValuePair<string, ClusteringResult<DataPoint>> clustering)
        {
            compute_series_points(clustering);
            int? k1 = (int?)KneedleAlgorithm.CalculateKneePoints(_chartXSeries, _chartYSeries, CurveDirection.Decreasing, Curvature.Counterclockwise, forceLinearInterpolation: false);
            int? k2 = (int?)KneedleAlgorithm.CalculateKneePoints(_chartXSeries, _chartYSeries, CurveDirection.Decreasing, Curvature.Clockwise, forceLinearInterpolation: false);

            return new int?[] { k1, k2 };
        }

        private void compute_series_points(KeyValuePair<string, ClusteringResult<DataPoint>> clustering)
        {
            _chartYSeriesFullPrecision = clustering.Value.Select(cs => cs.Dissimilarity).ToArray(); // Get Y values
            _chartYSeries = _chartYSeriesFullPrecision.Select(point => Math.Round(point, 2, MidpointRounding.ToEven)).ToArray();
            CurrentChartData = _chartYSeries;
            var intX = Enumerable.Range(0, _chartYSeries.Length).ToArray(); // Get X values
            _chartXSeries = intX.Select(point => (double)point).ToArray(); // Convert int array to double array
        }

        private SeriesCollection get_words_data()
        {
            var values = new ChartValues<int> { };

            var wordVec = dataCache.corpus.get_lexicon_word_counts();
            var nDataPoints = 500;
            Labels = new string[nDataPoints];
            Formatter = value => value.ToString("C");
            var count = 0;
            foreach(var item in wordVec)
            {
                if(count <= nDataPoints-1)
                {
                    values.Add(item.Value);
                    Labels[count] = item.Key;
                } else
                {
                    break;
                }
                count++;
            }
            Data.Add(new ColumnSeries
            {
                Title = "Word Count",
                Values = values
            });

            return Data;
        }

        public void get_chart_data(double[] levels)
        {
            var values = new ChartValues<ObservablePoint>() { };
            ChartLabels = new string[levels.Length];
            var count = 0;
            foreach (var level in levels)
            {
                values.Add(new ObservablePoint(count, Math.Round(level, 2, MidpointRounding.ToEven)));
                ChartLabels[count] = $"Level_{count + 1}";
                count++;
            }
            App.Current.Dispatcher.Invoke((Action)delegate { 
                ChartData.Add(new LineSeries
                {
                    Title = "Dissimilarity",
                    Values = values,
                    PointGeometrySize = 12
                });
            });
        }

        public void update_tree(ChartPoint p) // when a datapoint is selected in the chart
        {
            DataSource = new ObservableCollection<Item>();
            DataSource = get_tree_nodes((int?)p.X);
            SelectedDissimilarity = p.Y;
            SelectedClusteringLevel = (int)p.X;

            // Set the slider limits everytime a dissimilarity level is selected in the chart
            Ticks = _chartXSeries.Where(x => x < (int)p.X).Select(x => (int)x).ToArray(); // Get values less than dissimilarity and rescale to slider range
            MaxValue = Ticks.Length;
            DistanceSliderValue = MaxValue;
        }

        public void update_chart() // when the linkage is changed using the drop-down list
        {
            var kneepoint = _knees[LinkageMap[SelectedLinkage]]["k_c"];
            DataSource = get_tree_nodes(kneepoint);
            ChartData = new SeriesCollection { };
            get_chart_data(CurrentChartData);
        }


        private void RunCallback()
        {
            ThemesViewVisibility = "Collapsed";
            Linkages = new ObservableCollection<string>
                        {
                            "AVERAGE",
                            "COMPLETE",
                            "SINGLE",
                            "MIN-ENERGY",
                            "CENTROID",
                            "WARD"
                        };

            Thread generateThread = new Thread(new ThreadStart(create_clusters));
            generateThread.IsBackground = true;
            generateThread.Start();
        }

        double[] _currentChartData = { };
        double[] CurrentChartData { get { return _currentChartData; } set { _currentChartData = value; RaisePropertyChanged(); } }

        SeriesCollection _ChartData = new SeriesCollection { };
        private string[] _ChartLabels;
        public string[] ChartLabels { get { return _ChartLabels; } set { _ChartLabels = value; RaisePropertyChanged(); } }

        public SeriesCollection ChartData { get { return _ChartData; } set { _ChartData = value; RaisePropertyChanged(); } }


        SeriesCollection _data = new SeriesCollection {};

        public SeriesCollection Data { get { return _data; } set { _data = value; } }
        private string[] _labels;
        public string[] Labels { get { return _labels; } set { _labels = value; RaisePropertyChanged(); } }
        public Func<double, string> Formatter { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public double Dissimilarity { get; set; }
        public Color BackgroundColor { get; set; }
        public ObservableCollection<Item> Children { get; set; } = new ObservableCollection<Item>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class DissimilarityMetric : IDissimilarityMetric<DataPoint>
    {
        public double Calculate(DataPoint instance1, DataPoint instance2)
        {
            throw new NotImplementedException();
        }
    }
}

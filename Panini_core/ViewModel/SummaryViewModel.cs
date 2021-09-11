using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Panini;
using Panini.Commands;
using Panini.Models;
using Panini.ViewModel;
using TFIDF;

namespace Panini.ViewModel
{
	/// <summary>
	/// Manages the behavior of <c>Summary View</c>.
	/// </summary>
	public class SummaryViewModel : BaseViewModel
	{
		/// <summary>
		/// Single instance of <see cref="DataCache"/>.
		/// </summary>
		/// <value>Used for sharing resources between view models.</value>
		private readonly DataCache dataCache = DataCache.Instance;

		private TopicsInfo _topicsSummary;

		private NLPData _nlpDataInstance = new NLPData();

		private LinksInfo _linksSummary;

		private PlotModel _model = new PlotModel();

		private PlotModel _previewModel = new PlotModel();

		private BitmapSource _bmp;

		private bool _isHeatMapExpanded;

		private string _isHeatmapVisible = "Collapsed";

		private int _verticalSliderPosition = 0;

		private int _horizontalSliderPosition = 0;

		private ICommand _expandAll;

		private ICommand _collapseAll;

		private ICommand _run;

		private ICommand _generateHeatMapData;
		/// <summary>
		/// Summary info of all topics.
		/// </summary>
		/// <value>The <see cref="TopicsInfo"/> class instance that stores the summary information of topics in the corpus.</value>
		public TopicsInfo TopicsSummary
		{
			get
			{
				return _topicsSummary;
			}
			set
			{
				_topicsSummary = value;
				RaisePropertyChanged("TopicsSummary");
			}
		}
		/// <summary>
		/// NLP data of the performed ananlysis
		/// </summary>
		/// <value>The <see cref="NLPData"/> class instance that stores the NLP related information like tokenizer, 
		/// TFIDF vector length, similarity measure, number of total sentences and words in all topics.</value>
		public NLPData NLPDataInstance
		{
			get
			{
				return _nlpDataInstance;
			}
			set
			{
				_nlpDataInstance = value;
				RaisePropertyChanged("NLPDataInstance");
			}
		}

		/// <summary>
		/// Links data of all topics.
		/// </summary>
		/// <value>The <see cref="LinksInfo"/> class instance that stores the information 
		/// about links like total number of existing links (inline and related links), proposed links, matching links.</value>
		public LinksInfo LinksSummary
		{
			get
			{
				return _linksSummary;
			}
			set
			{
				_linksSummary = value;
				RaisePropertyChanged("LinksSummary");
			}
		}

		/// <summary>
		/// Plot model for the heatmap.
		/// </summary>
		/// <value>The handle of the plot for configuring axes and series of the plot.</value>
		public PlotModel Model
		{
			get
			{
				return _model;
			}
			set
			{
				_model = value;
				RaisePropertyChanged("Model");
			}
		}

		//public BitmapSource BMPSource
		//{
		//	get
		//	{
		//		return _bmp;
		//	}
		//	set
		//	{
		//		_bmp = value;
		//		RaisePropertyChanged("BMPSource");
		//	}
		//}

		/// <summary>
		/// Preview model.
		/// </summary>
		/// <value>The plot model corresponding to the preview part of the heatmap.</value>
		public PlotModel PreviewModel
		{
			get
			{
				return _previewModel;
			}
			set
			{
				_previewModel = value;
				RaisePropertyChanged("PreviewModel");
			}
		}

		/// <summary>
		/// State of the heatmap expander.
		/// </summary>
		/// <value>If <c>true</c> the headmap expander is expanded, else it is collpased.</value>
		public bool IsHeatMapExpanded
		{
			get
			{
				return _isHeatMapExpanded;
			}
			set
			{
				_isHeatMapExpanded = value;
				RaisePropertyChanged("IsHeatMapExpanded");
			}
		}

		/// <summary>
		/// Visibility of the heatmap expander.
		/// </summary>
		/// <value>If <c>true</c> the headmap expander is visibile, else it is collpased.</value>
		public string IsHeatmapVisible
		{
			get
			{
				return _isHeatmapVisible;
			}
			set
			{
				_isHeatmapVisible = value;
				RaisePropertyChanged("IsHeatmapVisible");
			}
		}

		/// <summary>
		/// Vertical slider postition.
		/// </summary>
		/// <value>The position of the vertical slider on the preview part of the heatmap.
		/// <para>This position is used for computing the part of the window to be displayed in the main heat map.</para></value>
		public int VerticalSliderPosition
		{
			get
			{
				return _verticalSliderPosition;
			}
			set
			{
				_verticalSliderPosition = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Horizontal slider postition.
		/// </summary>
		/// <value>The position of the horizontal slider on the preview part of the heatmap.
		/// <para>This position is used for computing the part of the window to be displayed in the main heat map.</para></value>
		public int HorizontalSliderPosition
		{
			get
			{
				return _horizontalSliderPosition;
			}
			set
			{
				_horizontalSliderPosition = value;
				RaisePropertyChanged();
			}
		}

        private float _sliderMax;

		/// <summary>
		/// Slider max value.
		/// </summary>
		/// <value>The maximum value for each slider on the heatmap preview.</value>
        public float SliderMax
        {
            get { return _sliderMax; }
            set { _sliderMax = value; RaisePropertyChanged(); }
        }


        private int _kernelSize = 10;
		/// <summary>
		/// Kernel size.
		/// </summary>
		/// <value>The window size of the heatmap.
		/// <para>The default value is <c>10</c>.</para></value>
        public int KernelSize
        {
            get { return _kernelSize; }
            set { _kernelSize = value; RaisePropertyChanged(); }
        }

		private string _isPreviewVisible;
		/// <summary>
		/// Visibility of the heatmap preview expander.
		/// </summary>
		/// <value>If <c>true</c> the headmap preview expander is visibile, else it is collpased.</value>
		public string IsPreviewVisible
        {
            get { return _isPreviewVisible; }
            set { _isPreviewVisible = value; RaisePropertyChanged(); }
        }

        private bool _isPreviewExpanded;
		/// <summary>
		/// State of the heatmap preview expander.
		/// </summary>
		/// <value>If <c>true</c> the headmap preview expander is expanded, else it is collpased.</value>
		public bool IsPreviewExpanded
        {
            get { return _isPreviewExpanded; }
            set { _isPreviewExpanded = value; RaisePropertyChanged(); }
        }


		private int _heatMapHeight = 800;
		/// <summary>
		/// Height of the heatmap.
		/// </summary>
		/// <value>The height of the main heatmap in pixels.</value>
        public int HeatmapHeight
        {
            get { return _heatMapHeight; }
            set { _heatMapHeight = value; RaisePropertyChanged(); }
        }

        private int _heatMapWidth = 750;
		/// <summary>
		/// Width of the heatmap.
		/// </summary>
		/// <value>The width of the main heatmap in pixels.</value>
		public int HeatMapWidth
        {
            get { return _heatMapWidth; }
            set { _heatMapWidth = value; RaisePropertyChanged(); }
        }



  //      public ICommand ExpandAll => _expandAll ?? (_expandAll = new ButtonCommandHandler(delegate
		//{
		//	ExpandAllCallback();
		//}, () => ExpandAllCanExecute));

		//public bool ExpandAllCanExecute => true;

		//public ICommand CollapseAll => _collapseAll ?? (_collapseAll = new ButtonCommandHandler(delegate
		//{
		//	CollapseAllCallBack();
		//}, () => CollapseAllCanExecute));

		//public bool CollapseAllCanExecute => true;

		/// <summary>
		/// Run command.
		/// </summary>
		/// <value>Command to start the generation of report and heatmap.</value>
		public ICommand Run => _run ?? (_run = new ButtonCommandHandler(delegate
		{
			RunCallback();
		}, () => RunCanExecute));

		/// <summary>
		/// Run command execution flag.
		/// </summary>
		/// <value>Determines if the <see cref="Run"/> command can execute in its current state.</value>
		public bool RunCanExecute => true;

		/// <summary>
		/// Command to generate heatmap data.
		/// </summary>
		/// <value>Generates the heatmap data and refreshes it in the view.</value>
		public ICommand GenerateHeatMapData => _generateHeatMapData ?? (_generateHeatMapData = new ButtonCommandHandler(delegate
		{
			App.Current.Dispatcher.InvokeAsync((Action)delegate { refresh_heat_map(); });
		}, () => true));


  //      private void ExpandAllCallback()
		//{
		//	try
		//	{
		//		TopicsSummary.IsExpanded = true;
		//		LinksSummary.IsExpanded = true;
		//		IsHeatMapExpanded = true;
		//		NLPDataInstance.IsExpanded = true;
		//		IsPreviewExpanded = true;
		//	}
		//	catch (NullReferenceException)
		//	{
		//		MessageBoxResult result = MessageBox.Show("Click Run button to generate the summary!", "Invalid Operation !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		//	}
		//}

		//private void CollapseAllCallBack()
		//{
		//	TopicsSummary.IsExpanded = false;
		//	LinksSummary.IsExpanded = false;
		//	IsHeatMapExpanded = false;
		//	NLPDataInstance.IsExpanded = false;
		//	IsPreviewExpanded = false;
		//}

		private void RunCallback()
		{
			if (TopicsSummary != null)
			{
				TopicsSummary.IsVisible = "Collapsed";
			}
			if (LinksSummary != null)
			{
				LinksSummary.IsVisible = "Collapsed";
			}
			if (IsHeatmapVisible != null)
			{
				IsHeatmapVisible = "Collapsed";
			}
			if (NLPDataInstance != null)
			{
				NLPDataInstance.IsVisible = "Collapsed";
			}
            if (KernelSize >= dataCache.corpus.topics.Count)
            {
				IsPreviewVisible = "Collapsed";
            }
			Thread generateTopicsThread = new Thread(generate_topics_info);
			generateTopicsThread.Start();
			Thread generateLinksThread = new Thread(generate_links_info);
			generateLinksThread.Start();
            if (KernelSize < dataCache.corpus.topics.Count) // If kernel size is less number of topics, generate preview.
            {
				Thread generatePreviewThread = new Thread(generate_preview_map);
				generatePreviewThread.Start();
            }
            else
            {
				IsPreviewExpanded = false;
				IsHeatmapVisible = "Collapsed";
            }
			Thread generatePlotThread = new Thread(generate_heat_map);
			generatePlotThread.Start();
			Thread generateNLPDataThread = new Thread(generate_nlp_data);
			generateNLPDataThread.Start();
		}
		
		/// <summary>
		/// Generates summary information on topics, which includes the total number of topics in the directory, number of valid and igonred topics.
		/// </summary>
		public void generate_topics_info()
		{
			TopicsSummary = new TopicsInfo();
			List<string> listOfFiles = dataCache.ListOfFiles;
			TopicsSummary.Path = dataCache.DirPath;
			TopicsSummary.NumTopics = listOfFiles.Count;
			foreach (string file in listOfFiles)
			{
				if (Topic.Is_valid(file, dataCache.corpus.ignoreData))
				{
					TopicsSummary.NumOfValidTopics++;
				}
				else
				{
					TopicsSummary.NumOfIgnoredTopics++;
				}
			}
			TopicsSummary.IsExpanded = true;
			TopicsSummary.IsVisible = "Visible";
		}

		/// <summary>
		/// Generates summary information on links in topics, which includes number of exiting links (inline and reltaed links).
		/// </summary>
		public void generate_links_info()
		{
			LinksSummary = new LinksInfo();
			LinksSummary.NumOfExistingRelLinks = get_count_of_all_rel_links();
			LinksSummary.NumOfExistingInlineLinks = get_count_of_all_inline_links();
			LinksSummary.NumOfExistingLinks = get_count_of_all_existing_links();
			LinksSummary.NumOfProposedLinks = get_count_of_all_proposed_links();
			LinksSummary.NumOfMatchingLinks = get_count_of_all_matching_links();
			LinksSummary.NumOfLinksNeedsIntegration = get_count_of_all_links_to_be_integrated();
			LinksSummary.NumOfObsoleteLinks = get_count_of_all_obsolete_links();
			LinksSummary.IsExpanded = true;
			LinksSummary.IsVisible = "Visible";
		}

		/// <summary>
		/// Counts the total related links in all the topics.
		/// </summary>
		/// <returns>The total number of related links in all the topics of the corpus.</returns>
		public int get_count_of_all_rel_links()
		{
			IEnumerable<List<string>> enumRelLinks = dataCache.corpus.concDict.Values.Select((Topic n) => n.relinks);
			List<string> allRelLinks = new List<string>();
			foreach (List<string> links in enumRelLinks)
			{
				allRelLinks.AddRange(links);
			}
			return allRelLinks.Count;
		}

		/// <summary>
		/// Counts all inline links in topics of the corpus.
		/// </summary>
		/// <returns>The total number of inline links present in all topics of the corpus.</returns>
		public int get_count_of_all_inline_links()
		{
			IEnumerable<List<string>> enumInlineLinks = dataCache.corpus.concDict.Values.Select((Topic n) => n.xrefs);
			List<string> allInlineLinks = new List<string>();
			foreach (List<string> links in enumInlineLinks)
			{
				allInlineLinks.AddRange(links);
			}
			return allInlineLinks.Count;
		}

		/// <summary>
		/// Counts all links proposed as similar topics in the results.
		/// </summary>
		/// <returns>The total number of inline links present in all topics of the corpus.</returns>
		public int get_count_of_all_proposed_links()
		{
			IEnumerable<int> enumLinkCount = dataCache.TopicCollection.Select((TopicItem n) => n.itemCollection.Count);
			int sum = 0;
			foreach (int count in enumLinkCount)
			{
				sum += count;
			}
			return sum;
		}


		/// <summary>
		/// Counts all links in topics of the corpus.
		/// </summary>
		/// <returns>The total number of links (inline + related) present in all topics of the corpus.</returns>
		public int get_count_of_all_existing_links()
		{
			return LinksSummary.NumOfExistingRelLinks + LinksSummary.NumOfExistingInlineLinks;
		}

		/// <summary>
		/// Counts all proposed as similar topics which are already linked in the topics.
		/// </summary>
		/// <returns>The total number of similar topic suggestions which are already linked in the topics.</returns>
		public int get_count_of_all_matching_links()
		{
			IEnumerable<IEnumerable<string>> enumMatchingLinks = dataCache.TopicCollection.Select((TopicItem n) => n.get_topic_names().Intersect(dataCache.corpus.concDict[n.Name].get_all_link_names()));
			int sum = 0;
			foreach (IEnumerable<string> links in enumMatchingLinks)
			{
				sum += links.Count();
			}
			return sum;
		}

		/// <summary>
		/// Counts the similar topics which can be integrated as links in the topics.
		/// </summary>
		/// <returns>The difference between the total number of suggested similar topics and the matching links that link to some of suggested similar topics.</returns>
		public int get_count_of_all_links_to_be_integrated()
		{
			IEnumerable<IEnumerable<string>> enumIntegrateLinks = dataCache.TopicCollection.Select((TopicItem n) => n.get_topic_names().Except(dataCache.corpus.concDict[n.Name].get_all_link_names()));
			int sum = 0;
			foreach (IEnumerable<string> links in enumIntegrateLinks)
			{
				sum += links.Count();
			}
			return sum;
		}

		/// <summary>
		/// Counts the obsolete links in all topics of the corpus.
		/// </summary>
		/// <returns>The total number of links which can be removed/replaced depending on better link proposals to other similar topics.</returns>
		public int get_count_of_all_obsolete_links()
		{
			IEnumerable<IEnumerable<string>> enumObseleteLinks = dataCache.TopicCollection.Select((TopicItem n) => dataCache.corpus.concDict[n.Name].get_all_link_names().Except(n.get_topic_names()));
			int sum = 0;
			foreach (IEnumerable<string> links in enumObseleteLinks)
			{
				sum += links.Count();
			}
			return sum;
		}

		/// <summary>
		/// Gets the heatmap data (axis labels and similarity scores) when the number of topics is less than or 
		/// equal to the window size in the preview. In this case, the preview heatmap is not generated as the 
		/// number of topics is low enough to fit in main heatmap.
		/// </summary>
		/// <returns>The axis labels and similarity scores for plotting the main heatmap.</returns>
		public Dictionary<string, object> get_full_heatmap_data()
		{
			string[] labels = dataCache.corpus.concDict.Keys.ToArray();
			double[,] data = new double[labels.Count(), labels.Count()];
			for (int i = 0; i < labels.Count(); i++)
			{
				for (int j = 0; j < labels.Count(); j++)
				{
					data[i, j] = dataCache.corpus.concDict[labels[i]].tfidf.similarityScores[labels[j]];
				}
			}
			Dictionary<string, object> dataDict = new Dictionary<string, object>();
			dataDict["hlabels"] = labels;
			dataDict["vlabels"] = labels;
			dataDict["data"] = data;
			return dataDict;
		}

		/// <summary>
		/// Generates and updates the heatmap preview.
		/// </summary>
		public void generate_preview_map()
        {
			Dictionary<string, object> plotData = get_full_heatmap_data();
			PreviewModel.DefaultColors = get_color_scheme();

			PreviewModel.Series.Clear();
			PreviewModel.Axes.Clear();
			PreviewModel.Annotations.Clear();

			PreviewModel.Axes.Add(new OxyPlot.Axes.LinearColorAxis
			{
				Palette = OxyPalette.Interpolate(10, PreviewModel.DefaultColors.ToArray()).Reverse(),
				Position = AxisPosition.Top,
				Minimum = 0.0,
				Maximum = 1.0,
				TicklineColor = OxyColors.Transparent,
				Key = "heatmapColorAxis",
				IsAxisVisible = false
			}) ;
			
            OxyPlot.Series.HeatMapSeries heatMapSeries = new OxyPlot.Series.HeatMapSeries
            {
				X0 = 0.0,
				X1 = dataCache.corpus.topics.Count - 1,
				Y0 = 0.0,
				Y1 = dataCache.corpus.topics.Count - 1,
				ColorAxisKey = "heatmapColorAxis",
				RenderMethod = HeatMapRenderMethod.Bitmap,
				Data = (double[,])plotData["data"]
			};
			PreviewModel.Series.Add(heatMapSeries);

			PreviewModel.Annotations.Add(new OxyPlot.Annotations.RectangleAnnotation()
			{
				MinimumX = 0,
				MinimumY = 0,
				MaximumX = KernelSize,
				MaximumY = KernelSize,
				Stroke = OxyColors.Black,
				StrokeThickness = 1,
				Fill = OxyColors.Transparent
			});
			IsPreviewVisible = "Visible";
			IsPreviewExpanded = true;
			PreviewModel.PlotView.InvalidatePlot();

   //         PngExporter exporter = new PngExporter();
			//App.Current.Dispatcher.Invoke((Action)delegate { BitmapSource BMPSource = exporter.ExportToBitmap(PreviewModel); });
			
		}

		/// <summary>
		/// Generates and updates the main heatmap.
		/// </summary>
		public void generate_heat_map()
		{
			Dictionary<string, object> plotData = new Dictionary<string, object>();
            if (KernelSize < dataCache.corpus.topics.Count)
            {
				plotData = get_partial_heatmap_data();
            }
            else
            {
				plotData = get_full_heatmap_data();
            }
			Model.DefaultColors = get_color_scheme();

			Model.Series.Clear();
			Model.Axes.Clear();
			Model.Axes.Add(new OxyPlot.Axes.LinearColorAxis
            {
				Palette = OxyPalette.Interpolate(10, Model.DefaultColors.ToArray()).Reverse(),
				Position = AxisPosition.Top,
				Minimum = 0.0,
				Maximum = 1.0,
				TicklineColor = OxyColors.Transparent,
				Key = "heatmapColorAxis"
			});

			Model.Axes.Add(new OxyPlot.Axes.CategoryAxis
            {
				Position = AxisPosition.Bottom,
				Key = "Source Topics",
				ItemsSource = (string[])plotData["hlabels"],
				Angle = 90.0,
			});

			Model.Axes.Add(new OxyPlot.Axes.CategoryAxis
            {
				Position = AxisPosition.Left,
				Key = "Target Topics",
				ItemsSource = (string[])plotData["vlabels"],
			});
			
            OxyPlot.Series.HeatMapSeries heatMapSeries = new OxyPlot.Series.HeatMapSeries
            {
				X0 = 0.0,
				X1 = KernelSize < dataCache.corpus.topics.Count ? KernelSize - 1 : dataCache.corpus.topics.Count-1,
				Y0 = 0.0,
				Y1 = KernelSize < dataCache.corpus.topics.Count ? KernelSize - 1 : dataCache.corpus.topics.Count - 1,
				XAxisKey = "Source Topics",
				YAxisKey = "Target Topics",
				ColorAxisKey = "heatmapColorAxis",
				RenderMethod = HeatMapRenderMethod.Rectangles,
				LabelFontSize = 0.2,
				Data = (double[,])plotData["data"]
			};

			Model.Series.Add(heatMapSeries);
			IsHeatMapExpanded = true;
			IsHeatmapVisible = "Visible";
			Model.PlotView.InvalidatePlot();

			// Only process if preview is visible
            if (KernelSize < dataCache.corpus.topics.Count) { 
				PreviewModel.Annotations.Clear();
				PreviewModel.Annotations.Add(new OxyPlot.Annotations.RectangleAnnotation()
				{
					MinimumX = (int)plotData["XMin"],
					MinimumY = (int)plotData["YMin"],
					MaximumX = (int)plotData["XMax"],
					MaximumY = (int)plotData["YMax"],
					Stroke = OxyColors.Black,
					StrokeThickness = 1,
					Fill = OxyColors.Transparent
				});
				PreviewModel.PlotView.InvalidatePlot();
			}
		}

        #region Refresh heat map and preview
		/// <summary>
		/// Refreshes both the heatmaps.
		/// </summary>
        public void refresh_heat_map()
		{
			if(KernelSize < dataCache.corpus.topics.Count)
            {
				generate_preview_map();
            }
			generate_heat_map();
		}
        #endregion

		/// <summary>
		/// Gets the colors corresponding to the scheme of the heatmap configured by the user in the settings view.
		/// </summary>
		/// <returns>The list of colors used by the color pallete of the heatmap.</returns>
        public List<OxyColor> get_color_scheme()
		{
			var scheme = dataCache.Config["colorScheme"];
			switch (scheme)
			{
				case "Hot":
					return new List<OxyColor>
				{
					OxyColor.FromRgb(217, 119, 119),
					OxyColors.Transparent
					//OxyColor.FromRgb(byte.MaxValue, 224, 224)
				};
				case "Cold":
					return new List<OxyColor>
				{
                    OxyColor.FromRgb(34, 191, 230),
					OxyColors.Transparent
                    //OxyColor.FromRgb(235, 251, byte.MaxValue)
				};
				case "HotCold":
					return new List<OxyColor>
				{
                    OxyColor.FromRgb(255, 61, 155),
					OxyColor.FromRgb(227, 227, 227),
					OxyColor.FromRgb(66, 192, 255)
				};
					
				case "Default":
					return new List<OxyColor>
				{
					OxyColor.FromRgb(59, 0, 168),
					OxyColors.Transparent
					//OxyColor.FromRgb(byte.MaxValue, 224, 224)
				};

				default:
					return new List<OxyColor>
				{
					OxyColor.FromRgb(59, 0, 168),
					OxyColors.Transparent
					//OxyColor.FromRgb(byte.MaxValue, 224, 224)
				};
			};
		}

		/// <summary>
		/// Generates the summary NLP information to be displayed in the summary view.
		/// </summary>
		public void generate_nlp_data()
		{
			NLPDataInstance.Tokenizer = "Regexp Tokenizer";
			NLPDataInstance.SimilarityMeasure = "Term-Frequency Inverse-Document-Frequency (TFIDF)";
			NLPDataInstance.TfIdfVectorLength = Lexicon.words.Count();
			NLPDataInstance.NumOfSentences = dataCache.corpus.concDict.Values.Select((Topic n) => n.sentCount).Sum();
			NLPDataInstance.NumOfTokens = Lexicon.words.Count();
			NLPDataInstance.IsExpanded = true;
			NLPDataInstance.IsVisible = "Visible";
		}

		/// <summary>
		/// Gets the partical data of the heatmap depending on the horizontal and vertical slider positions on the heatmap preview.
		/// </summary>
		/// <returns>The axis labels and similarity scores for the partial window to be displayed in the main heatmap.</returns>
		public Dictionary<string, object> get_partial_heatmap_data()
		{
			Dictionary<string, object> heatmapData = new Dictionary<string, object>();

			// Slider properties
			float TopicsIndexMin = 0;
			float numOfTopics = dataCache.corpus.concDict.Keys.Count;
			float SliderMin = 0;
			SliderMax = numOfTopics - KernelSize;

			float slope = (numOfTopics - KernelSize - TopicsIndexMin) / (SliderMax - SliderMin);

			int vIndex = (int) Math.Round(slope * VerticalSliderPosition);
			int hIndex = (int) Math.Round(slope * HorizontalSliderPosition);

			// Labels to be displayed on vertical and horizontal axes
			string[] vlabels = (string[])(heatmapData["vlabels"] = dataCache.corpus.concDict.Keys.ToList().GetRange(vIndex, KernelSize).ToArray());
			string[] hlabels = (string[])(heatmapData["hlabels"] = dataCache.corpus.concDict.Keys.ToList().GetRange(hIndex, KernelSize).ToArray());

			double[,] data = new double[KernelSize, KernelSize];

			for (int i = 0; i < KernelSize; i++)
			{
				for (int j = 0; j < KernelSize; j++)
				{
					data[i, j] = dataCache.corpus.concDict[vlabels[j]].tfidf.similarityScores[hlabels[i]];
				}
			}
			heatmapData["data"] = data;

			// Data to be used for drawing the rectangle annotation
			heatmapData["XMin"] = hIndex;
			heatmapData["XMax"] = hIndex + KernelSize;
			heatmapData["YMin"] = vIndex;
			heatmapData["YMax"] = vIndex + KernelSize;

			return heatmapData;
		}
	}
}

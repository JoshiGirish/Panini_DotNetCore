using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Panini.Pages;
using Panini.ViewModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Panini.Commands;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Collections;
using System.Collections.Specialized;

namespace Panini.ViewModel
{
    /// <summary>
    /// Manages the behavior of all views.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {

        private readonly AboutViewModel aboutVM;
        private readonly ConfigViewModel configVM;
        private readonly LoadViewModel loadVM;
        private readonly ResultsViewModel resultsVM;
        private readonly SummaryViewModel summaryVM;


        private AboutPage aboutPage = new AboutPage();
        private ConfigPage configPage = new ConfigPage();
        private LoadPage loadPage = new LoadPage();
        private ResultsPage resultsPage = new ResultsPage();
        private SummaryPage summaryPage = new SummaryPage();

        private Page _currentPage = new AboutPage();

        private readonly DataCache dataCache = DataCache.Instance;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; RaisePropertyChanged(); }
        }

        private ListViewItem _currentItem;

        /// <summary>
        /// Current view.
        /// </summary>
        /// <value>Represents the selected view.</value>
        public ListViewItem CurrentItem
        {
            get { return _currentItem; }
            set { _currentItem = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// State which stores the enabled views.
        /// </summary>
        /// <value>Collection of enabled views.</value>
        public ObservableCollection<string> ViewState
        {
            get { return dataCache.ViewState; }
            set { dataCache.ViewState = value; RaisePropertyChanged(); }
        }


        #region Switch View command callback
        private ICommand _switchView;
        /// <summary>
        /// Changes the current view depending on the selected navigation item.
        /// </summary>
        public ICommand SwitchView
        {
            get { return _switchView ?? (_switchView = new ParameterCommandHandler((obj) => SwitchPage(obj), () => { return true; })); }
        }

        private void SwitchPage(object name)
        {
            var listItemName = (string)name;

            switch (listItemName)
            {
                case "About":
                    CurrentPage = aboutPage;
                    break;

                case "Load":
                    CurrentPage = loadPage;
                    break;

                case "Config":
                    CurrentPage = configPage;
                    break;

                case "Results":
                    CurrentPage = resultsPage;
                    break;

                case "Summary":
                    CurrentPage = summaryPage;
                    break;

            }
        }
        #endregion

        #region Update View delegate
        /// <summary>
        /// Delegate for updating the view state, when <see cref="ViewState"/> collection changes.
        /// </summary>
        /// <param name="sender">Source element</param>
        /// <param name="e">Object that stores the event data.</param>
        public void update_view_status(object sender, NotifyCollectionChangedEventArgs e)
        {
            ViewState = dataCache.ViewState;
        }
        #endregion

        /// <summary>
        /// Initializes all <c>View Model</c> instances.
        /// </summary>
        public MainViewModel()
        {
            aboutVM = new AboutViewModel();
            configVM = new ConfigViewModel();
            loadVM = new LoadViewModel();
            resultsVM = new ResultsViewModel();
            summaryVM = new SummaryViewModel();

            dataCache.ViewState.CollectionChanged += update_view_status;
        }

        /// <summary>
        /// <c>About View</c> model.
        /// </summary>
        /// <value>View model for handling behavior of <c>About View</c>.</value>
        public BaseViewModel AboutViewModel { get { return aboutVM; } }
        /// <summary>
        /// <c>Base View</c> model.
        /// </summary>
        /// <value>View model from which other view models inherit.</value>
        public BaseViewModel baseViewModel { get { return configVM; } }
        /// <summary>
        /// <c>Laod View</c> model.
        /// </summary>
        /// <value>View model for handling behavior of <c>Load View</c>.</value>
        public BaseViewModel LoadViewModel { get { return loadVM; } }
        /// <summary>
        /// <c>Results View</c> model.
        /// </summary>
        /// <value>View model for handling behavior of <c>Results View</c>.</value>
        public BaseViewModel ResultsViewModel { get { return resultsVM; } }
        /// <summary>
        /// <c>Summary View</c> model.
        /// </summary>
        /// <value>View model for handling behavior of <c>Summary View</c>.</value>
        public BaseViewModel SummaryViewModel { get { return summaryVM; } }
    }
}

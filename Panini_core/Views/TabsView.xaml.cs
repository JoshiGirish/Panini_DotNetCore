using System.Windows;
using System.Windows.Controls;

namespace Panini.Views
{
    /// <summary>
    /// Interaction logic for TabsView.xaml
    /// </summary>
    public partial class TabsView : UserControl
    {
        public string SelectedTab
        {
            get { return (string)GetValue(ActiveTab); }
            set { SetValue(ActiveTab, value); switch_tab(value); }
        }
        public static readonly DependencyProperty ActiveTab = DependencyProperty.Register("SelectedTab", typeof(string), typeof(TabsView), new UIPropertyMetadata(""));

        public TabsView()
        {
            InitializeComponent();
        }

        public void switch_tab(string tabName)
        {
            navTabs.SelectedItem = tabName;
        }

    }
}

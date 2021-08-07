using System.Windows;
using System.Windows.Forms;
using Panini;

namespace Panini.Views
{
    /// <summary>
    /// Interaction logic for Input.xaml
    /// </summary>
    public partial class Input : System.Windows.Controls.UserControl
    {
        public Input()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folder = diag.SelectedPath;  //selected folder path
                dirPath.Text = folder;
            }
        }
    }
}

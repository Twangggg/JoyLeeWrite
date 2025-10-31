using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for AddInformationView.xaml
    /// </summary>
    public partial class AddInformationView : UserControl
    {
        public AddInformationView()
        {
            InitializeComponent();
        }

        public void DescriptionTextBox_TextChanged (object sender, EventArgs e)
        {

        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CreateSeries_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UploadArea_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

    }
}

using System;
using System.Collections.Generic;
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
    /// Interaction logic for AllSeriesView.xaml
    /// </summary>
    public partial class AllSeriesView : UserControl
    {
        public AllSeriesView()
        {
            InitializeComponent();
        }

        private void ViewSeriesDetail(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int seriesId)
            {
                MainWindow.MainVM.CurrentSeriesId = seriesId;
                MainWindow.navigate.navigatePage(new SeriesView());
                MainWindow.MainVM.addSeriesDetailViewModel(seriesId);
            }
        }
    }
}

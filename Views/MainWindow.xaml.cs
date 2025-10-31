using JoyLeeWrite.Services;
using JoyLeeWrite.ViewModels;
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
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NavigateService navigate { get; private set; }
        public static MainViewModel MainVM { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.Loaded += (s, e) =>
            {
                MainWindow.MainVM = new MainViewModel();
                this.DataContext = MainVM;
            };
            navigate = new NavigateService(MainFrame);
            navigate.navigatePage(new HomepageView());
        }
    }
}

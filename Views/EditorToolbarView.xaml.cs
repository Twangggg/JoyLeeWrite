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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for EditorToolbarView.xaml
    /// </summary>
    public partial class EditorToolbarView : UserControl
    {
        public EditorToolbarViewModel EditorToolbarVM { get; }
        public EditorToolbarView()
        {
            InitializeComponent();
        }
        public EditorToolbarView(TextFormattingService textService) : this()
        {
            EditorToolbarVM = new EditorToolbarViewModel(textService);
            this.DataContext = EditorToolbarVM;
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPopup.IsOpen = !ColorPopup.IsOpen;
        }

        private void QuickColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string colorHex)
            {
                var color = (Color)ColorConverter.ConvertFromString(colorHex);

                if (this.DataContext is EditorToolbarViewModel vm)
                {
                    vm.SelectedFontColor = color;
                }
                // Đóng popup
                ColorPopup.IsOpen = false;
            }
        }
    }
}

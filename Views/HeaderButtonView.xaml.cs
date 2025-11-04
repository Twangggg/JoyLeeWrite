using JoyLeeWrite.Services;
using JoyLeeWrite.ViewModels.WriteChapterViewModel;
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
    /// Interaction logic for HeaderButtonView.xaml
    /// </summary>
    public partial class HeaderButtonView : UserControl
    {
        public HeaderButtonViewModel HeaderButtonVM { get; }
        public HeaderButtonView()
        {
            InitializeComponent();
           
        }
        public HeaderButtonView(RichTextBox richTextBox): this()
        {
            HeaderButtonVM = new HeaderButtonViewModel(richTextBox);
            this.DataContext = HeaderButtonVM;
        }
    }
}

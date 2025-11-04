using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.ViewModels.WriteChapterVM;
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
    /// Interaction logic for WriteView.xaml
    /// </summary>
    public partial class WriteView : UserControl
    {
        public WriteView()
        {
            InitializeComponent();
            MainWindow.MainVM.addWriteChapterViewModel(EditorRichTextBox, MainWindow.MainVM.CurrentSeriesId, MainWindow.MainVM.CurrentChapterId);
        }
        public static readonly DependencyProperty ChapterIdProperty =
            DependencyProperty.Register(nameof(ChapterId), typeof(int), typeof(WriteView),
                new PropertyMetadata(0, OnChapterIdChanged));

        public int ChapterId
        {
            get => (int)GetValue(ChapterIdProperty);
            set => SetValue(ChapterIdProperty, value);
        }

        private static void OnChapterIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (WriteView)d;
            if (control.DataContext is WriteViewModel vm)
            {
                int newId = (int)e.NewValue;
                if (newId > 0)
                {
                    vm.LoadChapter(newId);
                }
            }
        }
    }
}

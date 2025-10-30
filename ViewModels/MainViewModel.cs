using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.ViewModels
{
    class MainViewModel
    {
        public EditorToolbarViewModel EditorToolbarVM { get; set; }
        public HeaderButtonViewModel HeaderButtonVM { get; set; }
        public RecentlyEditedViewModel RecentlyEditedVM { get; set; }
        public AllSeriesViewModel AllSeriesVM { get; set; }
        public MainViewModel(RichTextBox richTextBox)
        {
            TextFormattingService _textService = new TextFormattingService(richTextBox);
            HeaderButtonVM = new HeaderButtonViewModel(richTextBox);
            EditorToolbarVM = new EditorToolbarViewModel(_textService);
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
        }

        public MainViewModel()
        {
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
        }
    }
}

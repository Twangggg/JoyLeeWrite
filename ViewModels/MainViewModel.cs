using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.ViewModels
{
    public class MainViewModel
    {
        public EditorToolbarViewModel EditorToolbarVM { get; set; }
        public HeaderButtonViewModel HeaderButtonVM { get; set; }
        public RecentlyEditedViewModel RecentlyEditedVM { get; set; }
        public AllSeriesViewModel AllSeriesVM { get; set; }
        public AddInformationViewModel AddInformationVM { get; set; }
        public SeriesDetailViewModel SeriesDetailVM { get; set; }
        public CreateSeriesViewModel CreateSeriesVM { get; set; }
        public MainViewModel(RichTextBox richTextBox)
        {
            TextFormattingService _textService = new TextFormattingService(richTextBox);
            HeaderButtonVM = new HeaderButtonViewModel(richTextBox);
            EditorToolbarVM = new EditorToolbarViewModel(_textService);
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
            AddInformationVM = new AddInformationViewModel();
            CreateSeriesVM = new CreateSeriesViewModel();
        }

        public MainViewModel()
        {
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
            
        }

        public void addInformationViewModel()
        {
            AddInformationVM = new AddInformationViewModel();
        }
        public void addCreateSeriesViewModel()
        {
            CreateSeriesVM = new CreateSeriesViewModel();
        }

        public void addSeriesDetailViewModel(int seriesId)
        {
            SeriesDetailVM = new SeriesDetailViewModel(seriesId);
        }
    }
}

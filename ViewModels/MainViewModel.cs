using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public EditorToolbarViewModel EditorToolbarVM { get; set; }
        public HeaderButtonViewModel HeaderButtonVM { get; set; }
        public RecentlyEditedViewModel RecentlyEditedVM { get; set; }
        public AllSeriesViewModel AllSeriesVM { get; set; }
        public AddInformationViewModel AddInformationVM { get; set; }
        public SeriesDetailViewModel SeriesDetailVM { get; set; }
        public CreateSeriesViewModel CreateSeriesVM { get; set; }
        public AddChapterViewModel AddChapterVM { get; set; }
        public WriteViewModel WriteVM { get; set; }
        public AuthViewModel AuthVM { get; set; }
        public int CurrentSeriesId { get; set; }
        public int CurrentChapterId { get; set; }
        public int CurrentUserId { get; set; }
        public MainViewModel(RichTextBox richTextBox)
        {
            TextFormattingService _textService = new TextFormattingService(richTextBox);
            HeaderButtonVM = new HeaderButtonViewModel(richTextBox);
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
            CreateSeriesVM = new CreateSeriesViewModel();
        }

        public MainViewModel()
        {
            AuthVM = new AuthViewModel();
        }

        public void addHomepageViewModel()
        {
            CurrentPageTitle = "Homepage";
            SupPageTitle = "";
            RecentlyEditedVM = new RecentlyEditedViewModel();
            AllSeriesVM = new AllSeriesViewModel();
        }
        public void UpdateHomepage()
        {
            RecentlyEditedVM.UpdateRecentlyEditedVM();
            AllSeriesVM.UpdateAllSeriesVM();
        }
        public void addInformationViewModel(FormMode mode, int seriesId = 0)
        {
            if (mode == FormMode.Create)
            {
                CurrentPageTitle = "Create New Series";
                SupPageTitle = "Start your next story series. Fill in the details below to get started.";
            }
            else if (mode == FormMode.Edit)
            {
                CurrentPageTitle = "Edit Series Information";
                SupPageTitle = "Update your series details below.";
            }
            AddInformationVM = new AddInformationViewModel(mode, seriesId);
        }
        public void addCreateSeriesViewModel()
        {
            CreateSeriesVM = new CreateSeriesViewModel();
        }

        public void addSeriesDetailViewModel(int seriesId)
        {
            CurrentPageTitle = "Series Details";
            SupPageTitle = "Take a look at your created series.";
            SeriesDetailVM = new SeriesDetailViewModel(seriesId);
            AddChapterVM = new AddChapterViewModel(seriesId);
        }
        public void addWriteChapterViewModel(RichTextBox richTextBox, int seriesId, int chapterId)
        {
            TextFormattingService _textService = new TextFormattingService(richTextBox);
            EditorToolbarVM = new EditorToolbarViewModel(_textService, seriesId, chapterId);
            WriteVM = new WriteViewModel(_textService, seriesId, chapterId);
            HeaderButtonVM = new HeaderButtonViewModel(richTextBox);
        }
        private string _currentPageTitle;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set
            {
                _currentPageTitle = value;
            }
        }
        private string _supPageTitle;
        public string SupPageTitle
        {
            get => _supPageTitle;
            set
            {
                _supPageTitle = value;
            }
        }
    }
}

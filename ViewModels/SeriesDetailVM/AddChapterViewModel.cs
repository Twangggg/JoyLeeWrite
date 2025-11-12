using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels.WriteChapterVM
{
    public class AddChapterViewModel : INotifyPropertyChanged
    {
        private ChapterService chapterService;
        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private List<string> _statusList;
        public List<string> StatusList
        {
            get { return _statusList; }
            set
            {
                _statusList = value;
                OnPropertyChanged(nameof(StatusList));
            }
        }
        private int _chapterNumber;
        public int ChapterNumber
        {
            get { return _chapterNumber; }
            set
            {
                if (_chapterNumber != value)
                {
                    _chapterNumber = value;
                    OnPropertyChanged(nameof(ChapterNumber));
                }
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public ICommand SaveChapter { get; }
        public AddChapterViewModel(int seriesId)
        {
            chapterService = new ChapterService();
            Status = "Draft";
            StatusList = new List<string> { "Published", "Draft" };
            MainWindow.MainVM.CurrentSeriesId = seriesId;
            SaveChapter = new RelayCommand(_ => createChapter( seriesId));
        }

        private void createChapter( int seriesId)
        {
            if (!ValidateInput())
            {
                return;
            }
            Chapter chapter = new Chapter();
            chapter.Title = Title;
            chapter.ChapterNumber = ChapterNumber;
            chapter.Status = Status;
            chapter.SeriesId = seriesId;
            chapter.WordCount = 0;
            chapter.CreatedDate = DateTime.Now;
            chapter.LastModified = DateTime.Now;
            chapterService.createChapter(chapter);
            MainWindow.MainVM.CurrentChapterId = chapterService.getNewChapterId(seriesId);
            int chapterNumber = chapter.ChapterNumber;
            MainWindow.navigate.navigatePage(new WriteChapterView(chapter?.Title, chapterNumber));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool ValidateInput()
        {
            if (ChapterNumber <= 0)
            {
                MessageBox.Show("This chapter number is invalid", "Message");
                return false;
            }
            if (!chapterService.checkExistChapterNumber(ChapterNumber, MainWindow.MainVM.CurrentSeriesId))
            {
                MessageBox.Show("This chapter number is exist", "Message");
                return false;
            }
            if (string.IsNullOrEmpty(Title))
            {
                MessageBox.Show("Title can be not null", "Message");
            }
            return true;
        }
    }
}

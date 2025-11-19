using JoyLeeWrite.Commands;
using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels.HomepageViewModel
{
    public class RecentlyEditedViewModel
    {
        private ChapterService chapterService;
        public ObservableCollection<Chapter> RecentlyEdited { get; set; }
        public ICommand OpenChapterCommand { get; set; }
        public RecentlyEditedViewModel()
        {
            chapterService = new ChapterService();
            UpdateRecentlyEditedVM();
            OpenChapterCommand = new RelayCommand(OpenChapter);

        }
        private void OpenChapter(object parameter)
        {
            Chapter chapter = chapterService.getChapterById((int)parameter);
            MainWindow.MainVM.CurrentSeriesId = chapter.SeriesId;
            MainWindow.MainVM.CurrentChapterId = chapter.ChapterId;
            MainWindow.navigate.navigatePage(new WriteChapterView(chapter.Title, chapter.ChapterNumber));
        }
        public void UpdateRecentlyEditedVM()
        {
            var chapterList = chapterService.getRecentlyEditedChapters(6, MainWindow.MainVM.CurrentUser.UserId);
            RecentlyEdited = new ObservableCollection<Chapter>(chapterList);
        }
    }
}

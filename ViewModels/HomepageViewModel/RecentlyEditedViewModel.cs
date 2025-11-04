using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels.HomepageViewModel
{
    public class RecentlyEditedViewModel
    {
        private SeriesService seriesService;
        public ObservableCollection<Series> RecentlyEdited { get; set; }
        public RecentlyEditedViewModel()
        {
            seriesService = new SeriesService();
            var seriesList = seriesService.GetRecentlyEdited(6, MainWindow.MainVM.CurrentUser.UserId);
            RecentlyEdited = new ObservableCollection<Series>(seriesList);
        }

        public void UpdateRecentlyEditedVM()
        {
            var seriesList = seriesService.GetRecentlyEdited(6, MainWindow.MainVM.CurrentUser.UserId);
            RecentlyEdited = new ObservableCollection<Series>(seriesList);
        }
    }
}

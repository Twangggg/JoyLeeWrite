using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
   
    public class AllSeriesViewModel
    {
        private SeriesService seriesService;
        public ObservableCollection<Series> AllSeries { get; set; }
        public AllSeriesViewModel()
        {
            seriesService = new SeriesService();
            var seriesList = seriesService.GetAllSeries();
            AllSeries = new ObservableCollection<Series>(seriesList);
        }
    }
}

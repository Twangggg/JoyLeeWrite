﻿using JoyLeeWrite.Models;
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
        private ImageService imageService;
        private ChapterService chapterService;
        public ObservableCollection<Series> AllSeries { get; set; }
        public AllSeriesViewModel()
        {
            seriesService = new SeriesService();
            imageService = new ImageService();
            chapterService = new ChapterService();
            var seriesList = seriesService.GetAllSeries();
            foreach (Series series in seriesList)
            {
                if (!string.IsNullOrEmpty(series.CoverImgUrl))
                {
                    series.ChapterCount = chapterService.CountChaptersBySeriesId(series.SeriesId);
                    series.CoverImage = imageService.LoadAvifAsBitmap(series.CoverImgUrl);
                }
            }
            AllSeries = new ObservableCollection<Series>(seriesList);
        }
    }
}

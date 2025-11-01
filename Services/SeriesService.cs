﻿using JoyLeeWrite.Data;
using JoyLeeWrite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.Services
{

    class SeriesService
    {
        public DbContext dbContext;
        public SeriesService()
        {
            dbContext = new JoyLeeWriteDbContext();
        }

        public List<string> GetAllSeriesNames()
        {
            try
            {
                var seriesNames = dbContext.Set<Series>()
                    .Select(s => s.Title)
                    .ToList();
                return seriesNames;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return new List<string>();
            }
        }

        public List<Series> GetRecentlyEdited(int limit)
        {
            List<Series> seriesList = new List<Series>();
            try
            {
                seriesList = dbContext.Set<Series>()
                    .OrderByDescending(s => s.LastModified)
                    .Take(limit)
                    .ToList();
                foreach (Series series in seriesList)
                {
                    series.ChapterCount = dbContext.Set<Chapter>()
                        .Count(c => c.SeriesId == series.SeriesId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving recently edited series: {ex.Message}");
            }
            return seriesList;
        }

        public List<Series> GetAllSeries()
        {
            List<Series> seriesList = new List<Series>();
            try
            {
                seriesList = dbContext.Set<Series>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving all series: {ex.Message}");
            }
            return seriesList;
        }

        public bool AddSeries(Series newSeries, List<Category> categories)
        {
            try
            {
                dbContext.Set<Series>().Add(newSeries);
                dbContext.SaveChanges();
                foreach (var category in categories)
                {
                    dbContext.Attach(category);
                    newSeries.Categories.Add(category);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new series: {ex.Message}");
                return false;
            }
        }

        public Series GetSeriesById(int seriesId)
        {
            Series series = new Series();
            try
            {
                series = dbContext.Set<Series>()
                    .Include(s => s.Categories)
                    .FirstOrDefault(s => s.SeriesId == seriesId);
                if (series != null)
                {
                    series.ChapterCount = dbContext.Set<Chapter>()
                        .Count(c => c.SeriesId == series.SeriesId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving series by ID: {ex.Message}");
            }
            return series;
        }
    }
}

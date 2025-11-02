using JoyLeeWrite.Data;
using JoyLeeWrite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.Services
{
    class CategoryService
    {
        private readonly DbContext dbContext;

        public CategoryService()
        {
            this.dbContext = new JoyLeeWriteDbContext();
        }

        public List<Category> GetCategories()
        {
            return dbContext.Set<Category>().ToList();
        }

        public List<Category> GetCategories(List<int> categoryIds)
        {
            return dbContext.Set<Category>()
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToList();
        }
        public List<Category> GetCategoriesBySeriesId(int seriesId)
        {
            return dbContext.Set<Category>()
                .Where(c => c.Series.Any(s => s.SeriesId == seriesId))
                .ToList();
        }

        public List<Category> GetCategoriesNotInSeries(int seriesId)
        {
            return dbContext.Set<Category>()
                .Where(c => c.Series.All(s => s.SeriesId != seriesId))
                .ToList();
        }
    }
}

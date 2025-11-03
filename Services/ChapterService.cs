using JoyLeeWrite.Data;
using JoyLeeWrite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JoyLeeWrite.Services
{
    class ChapterService
    {
        public DbContext dbContext;
        public ChapterService()
        {
            dbContext = new JoyLeeWriteDbContext();
        }

        public bool canSaveChapter(string content)
        {
            return !string.IsNullOrWhiteSpace(content);
        }

        public void saveChapter(string content, int chapterId)
        {
            try
            {
                Chapter chapter = dbContext.Set<Chapter>().Find(chapterId);

                if (chapter == null)
                {
                    MessageBox.Show("Không tìm thấy chương cần lưu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                chapter.Content = content;
                chapter.LastModified = DateTime.Now;
                dbContext.SaveChanges();
                MessageBox.Show("Lưu chương thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu chương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<Chapter> GetChaptersBySeriesId(int seriesId)
        {
            return dbContext.Set<Chapter>()
                .Where(c => c.SeriesId == seriesId)
                .OrderBy(c => c.ChapterNumber)
                .ToList();
        }
        public int CountChaptersBySeriesId(int seriesId)
        {
            return dbContext.Set<Chapter>()
                .Count(c => c.SeriesId == seriesId);
        }

        public Chapter getChapterById (int chapterId)
        {
            return dbContext.Set<Chapter>()
                .FirstOrDefault(c => c.ChapterId == chapterId);
        }

        public bool createChapter(Chapter chapter)
        {

            try
            {
                dbContext.Set<Chapter>().Add(chapter);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int getNewChapterId()
        {
            return dbContext.Set<Chapter>().OrderByDescending(c => c.CreatedDate).First().ChapterId;
        }

        public bool checkExistChapterNumber(int chapterNumber)
        {
            return dbContext.Set<Chapter>().Any(c => c.ChapterNumber == chapterNumber) == false; ;
        }
    }
}

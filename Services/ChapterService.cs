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
        private JoyLeeWriteDbContext CreateContext()
        {
            return new JoyLeeWriteDbContext();
        }

        public bool canSaveChapter(string content)
        {
            return !string.IsNullOrWhiteSpace(content);
        }

        public void saveChapter(string content, int chapterId, int wordCount)
        {
            using (var dbContext = CreateContext())
            {
                try
                {
                    Chapter chapter = dbContext.Set<Chapter>().Find(chapterId);

                    if (chapter == null)
                    {
                        MessageBox.Show("Error when saving!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    chapter.Content = content;
                    chapter.WordCount = wordCount;
                    chapter.LastModified = DateTime.Now;

                    dbContext.Entry(chapter).State = EntityState.Modified;

                    int result = dbContext.SaveChanges();

                    if (result > 0)
                    {
                        MessageBox.Show("Save Successful!", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No change in text!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}\nInner: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public List<Chapter> GetChaptersBySeriesId(int seriesId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>()
                    .Where(c => c.SeriesId == seriesId)
                    .OrderBy(c => c.ChapterNumber)
                    .ToList();
            }
        }

        public int CountChaptersBySeriesId(int seriesId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>()
                    .Count(c => c.SeriesId == seriesId);
            }
        }

        public Chapter getChapterById(int chapterId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>()
                    .FirstOrDefault(c => c.ChapterId == chapterId);
            }
        }

        public bool createChapter(Chapter chapter)
        {
            using (var dbContext = CreateContext())
            {
                try
                {
                    dbContext.Set<Chapter>().Add(chapter);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tạo chương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public int getNewChapterId(int seriesId)
        {
            using (var dbContext = CreateContext())
            {
                var chapter = dbContext.Set<Chapter>()
                    .Where(c => c.SeriesId == seriesId)
                    .OrderByDescending(c => c.CreatedDate)
                    .FirstOrDefault();

                return chapter?.ChapterId ?? 0;
            }
        }

        public bool checkExistChapterNumber(int chapterNumber, int seriesId)
        {
            using (var dbContext = CreateContext())
            {
                return !dbContext.Set<Chapter>()
                    .Any(c => c.ChapterNumber == chapterNumber && c.SeriesId == seriesId);
            }
        }

        public bool DeleteChapterById(int chapterId)
        {
            using (var dbContext = CreateContext())
            {
                try
                {
                    var chapter = dbContext.Set<Chapter>()
                        .FirstOrDefault(c => c.ChapterId == chapterId);

                    if (chapter != null)
                    {
                        dbContext.Set<Chapter>().Remove(chapter);
                        dbContext.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa chương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        public int GetTotalChapters(int userId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>()
            .Where(c => c.Series.AuthorId == userId)
            .Count();
            }
        }

        public int GetTotalWords(int userId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>().Where(c => c.Series.AuthorId == userId)
                    .Sum(c => c.WordCount);
            }
        }

        public List<Chapter> getRecentlyEditedChapters(int limit, int userId)
        {
            using (var dbContext = CreateContext())
            {
                return dbContext.Set<Chapter>().Where(c => c.Series.AuthorId == userId).OrderByDescending(c => c.LastModified).Take(limit).ToList();
            }
        }
    }
}
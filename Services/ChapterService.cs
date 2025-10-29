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

        public void saveChapter(string content)
        {
            try
            {
                int chapterId = 1; // Giả sử chúng ta đang lưu chương có ID = 1
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
    }
}

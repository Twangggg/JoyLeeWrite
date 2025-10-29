using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JoyLeeWrite.Utils
{
    internal class RichTextHelper
    {
        /// <summary>
        /// Lấy toàn bộ nội dung trong RichTextBox.
        /// </summary>
        /// <param name="rtb">Đối tượng RichTextBox cần lấy nội dung</param>
        /// <param name="includeFormatting">
        /// - false: Trả về văn bản thuần (plain text)
        /// - true: Trả về nội dung có định dạng (XAML)
        /// </param>
        /// <returns>Chuỗi nội dung RichTextBox</returns>
        public static string GetRichText(RichTextBox richTextBox, bool includeFormatting = false)
        {
            if (richTextBox == null || richTextBox.Document == null)
                return string.Empty;

            TextRange range = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd
            );

            if (!includeFormatting)
            {
                // Trả về nội dung văn bản thuần (không định dạng)
                return range.Text.TrimEnd();
            }
            else
            {
                // Trả về nội dung có định dạng (XAML)
                using (MemoryStream stream = new MemoryStream())
                {
                    range.Save(stream, DataFormats.Xaml);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}

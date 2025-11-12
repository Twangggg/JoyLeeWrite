using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.ViewModels.WriteChapterVM
{
    public class WriteViewModel : INotifyPropertyChanged
    {
        private readonly TextFormattingService _textService;
        private readonly RichTextBox richTextBox;
        private ChapterService _chapterService;
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
        private string _wordCount;
        public string WordCount
        {
            get => _wordCount;
            set
            {
                if (_wordCount != value)
                {
                    _wordCount = value;
                    OnPropertyChanged();
                }
            }
        }
        public WriteViewModel(TextFormattingService textService, RichTextBox richTextBox, int seriesId, int chapterId)
        {
            this.richTextBox = richTextBox;
            _textService = textService;
            _chapterService = new ChapterService();
            Chapter chapter = _chapterService.getChapterById(chapterId);
            Content = chapter?.Content ?? "<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
           "<Paragraph FontSize='16'>Bắt đầu <Bold>viết truyện</Bold> của bạn ở đây...</Paragraph>" +
           "</FlowDocument>";
            richTextBox.TextChanged += RichTextBox_TextChanged;
            UpdateWordCount();
        }
        private int GetWordCount()
        {
            return RichTextHelper.GetRichText(richTextBox, false).Trim().Replace(" ", "").Length;
        }
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWordCount();
        }

        private void UpdateWordCount()
        {
            int count = GetWordCount();
            WordCount = $"Word Count: {count}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public void LoadChapter(int chapterId)
        {
            Chapter chapter = _chapterService.getChapterById(chapterId);
            Content = chapter?.Content ?? "Chưa có nội dung.";
            UpdateWordCount();
        }
    }
}

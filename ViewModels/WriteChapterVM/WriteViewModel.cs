using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels.WriteChapterVM
{
    public class WriteViewModel : INotifyPropertyChanged
    {
        private readonly TextFormattingService _textService;
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
        public WriteViewModel(TextFormattingService textService, int seriesId, int chapterId)
        {
            _textService = textService;
            _chapterService = new ChapterService();
            Chapter chapter = _chapterService.getChapterById(chapterId);
            Content = chapter?.Content ?? "<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
           "<Paragraph FontSize='16'>Bắt đầu <Bold>viết truyện</Bold> của bạn ở đây...</Paragraph>" +
           "</FlowDocument>";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public void LoadChapter(int chapterId)
        {
            Chapter chapter = _chapterService.getChapterById(chapterId);
            Content = chapter?.Content ?? "Chưa có nội dung.";
        }
    }
}

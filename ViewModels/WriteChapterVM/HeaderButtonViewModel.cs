using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using JoyLeeWrite.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels.WriteChapterViewModel
{
    public class HeaderButtonViewModel
    {
        private readonly RichTextBox richTextBox;
        private readonly ChapterService chapterService;
        public ICommand SaveChapter { get; }
        public HeaderButtonViewModel(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            chapterService = new ChapterService();
            
            SaveChapter = new RelayCommand(_ => chapterService.saveChapter(RichTextHelper.GetRichText(richTextBox, true), MainWindow.MainVM.CurrentChapterId, getWordCount()), _ => chapterService.canSaveChapter(RichTextHelper.GetRichText(richTextBox, false)));
        }
        private int getWordCount()
        {
            return RichTextHelper.GetRichText(richTextBox, false).Trim().Replace(" ", "").Length;
        }

    }
}

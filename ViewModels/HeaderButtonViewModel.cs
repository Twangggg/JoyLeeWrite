using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace JoyLeeWrite.ViewModels
{
    public class HeaderButtonViewModel
    {
        private readonly RichTextBox richTextBox;
        private readonly ChapterService chapterService;
        public ICommand SaveChapter { get; }
        public HeaderButtonViewModel(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            this.chapterService = new ChapterService();
            SaveChapter = new RelayCommand(_ => chapterService.saveChapter(RichTextHelper.GetRichText(richTextBox, true)), _ => chapterService.canSaveChapter(RichTextHelper.GetRichText(richTextBox, false)));
        }

    }
}

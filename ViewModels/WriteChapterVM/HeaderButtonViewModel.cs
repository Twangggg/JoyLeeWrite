using DocumentFormat.OpenXml.Packaging;
using JoyLeeWrite.Commands;
using JoyLeeWrite.Services;
using JoyLeeWrite.Utils;
using JoyLeeWrite.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        public ICommand ImportCommand { get; }
        public HeaderButtonViewModel(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            chapterService = new ChapterService();

            ImportCommand = new RelayCommand(ImportWordFile);
            SaveChapter = new RelayCommand(_ => chapterService.saveChapter(RichTextHelper.GetRichText(richTextBox, true), MainWindow.MainVM.CurrentChapterId, getWordCount()), _ => chapterService.canSaveChapter(RichTextHelper.GetRichText(richTextBox, false)));
        }
        private int getWordCount()
        {
            return RichTextHelper.GetRichText(richTextBox, false).Trim().Replace(" ", "").Length;
        }
        public string ReadWordFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return string.Empty;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                return body.InnerText;
            }
        }
        private void ImportWordFile(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Word Documents|*.docx",
                Title = "Chọn file Word để import"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string text = ReadWordFile(openFileDialog.FileName);
                if (!string.IsNullOrEmpty(text))
                {
                    RichTextHelper.SetPlainText(richTextBox, text); 
                }
            }
        }
    }
}

using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.ViewModels
{
    public class MainViewModel
    {
        public RichTextBox _richTextBox;
        public FileViewModel FileVM { get; set; }
        public EditorToolbarViewModel EditorToolbarVM { get; set; }

        public MainViewModel(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            TextFormattingService textService = new TextFormattingService(_richTextBox);
            HandleFileService fileService = new HandleFileService();

            EditorToolbarVM = new EditorToolbarViewModel(textService);
            FileVM = new FileViewModel(fileService);
        }
    }
}

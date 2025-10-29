using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    class MainViewModel
    {
        public EditorToolbarViewModel EditorToolbarVM { get; set; }

        public MainViewModel(TextFormattingService _textService)
        {
            EditorToolbarVM = new EditorToolbarViewModel(_textService);
        }

    }
}

using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    public class FileViewModel
    {
        private readonly HandleFileService _fileService;
        public FileViewModel(HandleFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }
    }
}

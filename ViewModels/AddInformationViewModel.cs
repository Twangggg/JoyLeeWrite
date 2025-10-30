using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    class AddInformationViewModel
    {
        private SeriesService seriesService;
        public AddInformationViewModel()
        {
            seriesService = new SeriesService();
        }
    }
}

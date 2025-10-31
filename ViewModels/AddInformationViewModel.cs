using JoyLeeWrite.Models;
using JoyLeeWrite.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyLeeWrite.ViewModels
{
    public class AddInformationViewModel
    {
        private CategoryService categoryService;
        private SeriesService seriesService;
        public ObservableCollection<Category> Categories { get; set; }

        public AddInformationViewModel()
        {
            seriesService = new SeriesService();
            categoryService = new CategoryService();
            List<Category> categoryList = categoryService.GetCategories();
            Categories = new ObservableCollection<Category>(categoryList);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoyLeeWrite.Views
{
    /// <summary>
    /// Interaction logic for AddChapterView.xaml
    /// </summary>
    public partial class AddChapterView : UserControl
    {
        public event Action<string, int>? ChapterAdded;
        public event Action? Cancelled;
        public AddChapterView()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            int.TryParse(txtNumber.Text, out int number);

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Vui lòng nhập tên chapter.");
                return;
            }
            ChapterAdded?.Invoke(title, number);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancelled?.Invoke();
        }
    }
}

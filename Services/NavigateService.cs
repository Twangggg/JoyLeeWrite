using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JoyLeeWrite.Services
{
    public class NavigateService
    {
        private readonly Frame mainFrame;
        public NavigateService(Frame frame)
        {
            mainFrame = frame ?? throw new ArgumentNullException(nameof(mainFrame)); 
        }

        public void navigatePage (Page page)
        {
            mainFrame.Navigate(page);
        }

        public void goBack()
        {
            if (mainFrame.CanGoBack)
            {
                mainFrame.GoBack();
            }
        }
        public void ClearHistory()
        {
            while (mainFrame.CanGoBack)
                mainFrame.RemoveBackEntry();
        }
    }
}

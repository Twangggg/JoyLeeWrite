using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace JoyLeeWrite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static RichTextBox EditorRichTextBox { get; internal set; }
    }

}

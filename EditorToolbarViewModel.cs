using JoyLeeBookWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace JoyLeeBookWriter
{
    public class EditorToolbarViewModel
    {
        private MainWindow _mainWindow;
        private readonly RichTextBox _editor;
        private TextSelection Selection => _editor?.Selection;

        public ICommand ToggleBoldCommand { get; }
        public ICommand ToggleItalicCommand { get; }
        public ICommand ToggleUnderlineCommand { get; }
        public ICommand ToggleUpSize { get; }
        public ICommand ToggleDownSize { get; }

        public EditorToolbarViewModel(MainWindow mainWindow, RichTextBox editor)
        {
            _editor = editor;
            _mainWindow = mainWindow;
            ToggleBoldCommand = new RelayCommand(_ => ToggleBold(), _ => CanFormat());
            ToggleItalicCommand = new RelayCommand(_ => ToggleItalic(), _ => CanFormat());
            ToggleUnderlineCommand = new RelayCommand(_ => ToggleUnderline(), _ => CanFormat());
            ToggleUpSize = new RelayCommand(_ => UpSize(), _ => CanFormat());
            ToggleDownSize = new RelayCommand(_ => DownSize(), _ => CanFormat());
        }

        private bool CanFormat()
        {
            return _editor != null && _editor.Selection != null;
        }

        private void ToggleBold()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }
            ApplyStyle(TextElement.FontWeightProperty, FontWeights.Bold, FontWeights.Normal);
        }

        private void ToggleItalic()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }
            ApplyStyle(TextElement.FontStyleProperty, FontStyles.Italic, FontStyles.Normal);
        }

        private void ToggleUnderline()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }

            var current = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            bool isUnderlined = (current != DependencyProperty.UnsetValue && current == TextDecorations.Underline);
            Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, isUnderlined ? null : TextDecorations.Underline);
        }

        private void ApplyStyle(DependencyProperty property, object activeValue, object normalValue)
        {
            if (Selection == null) return;

            var current = Selection.GetPropertyValue(property);
            if (current != DependencyProperty.UnsetValue && current.Equals(activeValue))
                Selection.ApplyPropertyValue(property, normalValue);
            else
                Selection.ApplyPropertyValue(property, activeValue);
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Selection == null || _editor == null)
                return;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.B:
                        ToggleBold();
                        e.Handled = true;
                        break;
                    case Key.I:
                        ToggleItalic();
                        e.Handled = true;
                        break;
                    case Key.U:
                        ToggleUnderline();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void UpSize()
        {
            if (_editor.Selection == null) return;

            var current = Selection.GetPropertyValue(TextElement.FontSizeProperty);

            double currentSize;
            if (current == DependencyProperty.UnsetValue)
            {
                currentSize = _editor.FontSize;
            }
            else
            {
                currentSize = Convert.ToDouble(current);
            }

            double newSize = currentSize + 2;

            if (newSize > 72) newSize = 72;

            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            _mainWindow.FontSize.Text = $"{newSize:0}";
        }

        private void DownSize()
        {
            if (_editor.Selection == null) return;

            var current = Selection.GetPropertyValue(TextElement.FontSizeProperty);

            double currentSize;
            if (current == DependencyProperty.UnsetValue)
            {
                currentSize = _editor.FontSize;
            }
            else
            {
                currentSize = Convert.ToDouble(current);
            }

            double newSize = currentSize - 2;

            if (newSize < 8) newSize = 8;

            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            _mainWindow.FontSize.Text = $"{newSize:0}";
        }
    }
}
using JoyLeeBookWriter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using JoyLeeWrite.Commands;

namespace JoyLeeWrite.ViewModels
{
    public class EditorToolbarViewModel : INotifyPropertyChanged
    {
        // Truyền MainWindow để có thể sử dụng khi cần
        private MainWindow _mainWindow;
        // Dùng RichTextBox để hỗ trợ định dạng văn bản
        private readonly RichTextBox _editor;
        // Dùng để lấy đoạn văn bản được người dùng bôi đen, chọn
        private TextSelection Selection => _editor?.Selection;
        // Property màu chữ
        private Color _selectedFontColor = Colors.Black;
        // Các lớp ICommand để liên kết với các nút trên thanh công cụ thông qua binding
        public ICommand ToggleBoldCommand { get; }
        public ICommand ToggleItalicCommand { get; }
        public ICommand ToggleUnderlineCommand { get; }
        public ICommand ToggleUpSize { get; }
        public ICommand ToggleDownSize { get; }
        public ICommand ToggleAlignLeft { get; }
        public ICommand ToggleAlignCenter { get; }
        public ICommand ToggleAlignRight { get; }
        public ICommand ToggleAlignJustify { get; }

        public EditorToolbarViewModel(MainWindow mainWindow, RichTextBox editor)
        {
            _editor = editor;
            _mainWindow = mainWindow;
            ToggleBoldCommand = new RelayCommand(_ => ToggleBold(), _ => CanFormat());
            ToggleItalicCommand = new RelayCommand(_ => ToggleItalic(), _ => CanFormat());
            ToggleUnderlineCommand = new RelayCommand(_ => ToggleUnderline(), _ => CanFormat());
            ToggleUpSize = new RelayCommand(_ => UpSize(), _ => CanFormat());
            ToggleDownSize = new RelayCommand(_ => DownSize(), _ => CanFormat());
            ToggleAlignLeft = new RelayCommand(_ => AlignLeft(), _ => CanFormat());
            ToggleAlignCenter = new RelayCommand(_ => AlignCenter(), _ => CanFormat());
            ToggleAlignRight = new RelayCommand(_ => AlignRight(), _ => CanFormat());
            ToggleAlignJustify = new RelayCommand(_ => AlignJustify(), _ => CanFormat());
        }
        // Kiểm tra xem có thể định dạng văn bản hay không
        private bool CanFormat()
        {
            return _editor != null && _editor.Selection != null;
        }
        // Bôi đậm đoạn văn bản được chọn
        private void ToggleBold()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }
            ApplyStyle(TextElement.FontWeightProperty, FontWeights.Bold, FontWeights.Normal);
        }
        // In nghiêng đoạn văn bản được chọn
        private void ToggleItalic()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }
            ApplyStyle(TextElement.FontStyleProperty, FontStyles.Italic, FontStyles.Normal);
        }
        // Gạch chân đoạn văn bản được chọn
        private void ToggleUnderline()
        {
            if (_editor.Selection == null)
            {
                _editor.Focus();
                if (_editor.Selection == null) return;
            }

            var current = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            bool isUnderlined = current != DependencyProperty.UnsetValue && current == TextDecorations.Underline;
            Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, isUnderlined ? null : TextDecorations.Underline);
        }
        // Áp dụng hoặc loại bỏ định dạng dựa trên giá trị hiện tại
        private void ApplyStyle(DependencyProperty property, object activeValue, object normalValue)
        {
            if (Selection == null) return;

            var current = Selection.GetPropertyValue(property);
            if (current != DependencyProperty.UnsetValue && current.Equals(activeValue))
                Selection.ApplyPropertyValue(property, normalValue);
            else
                Selection.ApplyPropertyValue(property, activeValue);
        }
        // Xử lý sự kiện phím tắt
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
        // Tăng kích thước chữ của đoạn văn bản được chọn
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
        // Giảm kích thước chữ của đoạn văn bản được chọn
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
        // Căn chỉnh đoạn văn bản được chọn
        private void AlignSelection (TextAlignment alignment)
        {
            if (_editor.Selection == null) return;
            TextPointer start = _editor.Selection.Start;
            TextPointer end = _editor.Selection.End;

            var paragraphs = _editor.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.ContentStart.CompareTo(end) < 0 &&
                            p.ContentEnd.CompareTo(start) > 0);

            foreach (Paragraph p in paragraphs)
            {
                p.TextAlignment = alignment;
            }
        }
        private void AlignLeft()
        {
           AlignSelection(TextAlignment.Left);
        }

        private void AlignCenter()
        {
            AlignSelection(TextAlignment.Center);
        }

        private void AlignRight()
        {
            AlignSelection(TextAlignment.Right);
        }

        private void AlignJustify()
        {
            AlignSelection(TextAlignment.Justify);
        }
        public Color SelectedFontColor
        {
            get => _selectedFontColor;
            set
            {
                if (_selectedFontColor != value)
                {
                    _selectedFontColor = value;
                    OnPropertyChanged();
                    ApplyFontColor(value); 
                }
            }
        }
        // Hàm apply màu cho text đang chọn
        private void ApplyFontColor(Color color)
        {
            if (_editor == null) return;
            var brush = new SolidColorBrush(color);
            // Nếu có text được chọn
            if (_editor.Selection != null && !_editor.Selection.IsEmpty)
            {
                _editor.Selection.ApplyPropertyValue(
                    TextElement.ForegroundProperty,
                    brush
                );
            }
            else
            {
                // Nếu không có text được chọn, set màu cho text sẽ gõ tiếp theo
                // Lấy vị trí con trỏ hiện tại
                TextPointer caretPosition = _editor.CaretPosition;
                // Tạo TextRange tại vị trí con trỏ để set thuộc tính
                TextRange range = new TextRange(caretPosition, caretPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                // Hoặc có thể set trực tiếp cho Selection (cách an toàn hơn)
                _editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            }
        }
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
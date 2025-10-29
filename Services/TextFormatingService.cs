using JoyLeeBookWriter;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace JoyLeeWrite.Services
{
    public class TextFormattingService
    {
        private readonly RichTextBox _editor;

        public TextFormattingService(RichTextBox editor)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
        }

        private TextSelection Selection => _editor?.Selection;

        private bool HasSelection => Selection != null && !Selection.IsEmpty;

        public bool CanFormat() => _editor != null && _editor.Selection != null;

        public void ToggleBold() => ApplyStyle(TextElement.FontWeightProperty, FontWeights.Bold, FontWeights.Normal);
        public void ToggleItalic() => ApplyStyle(TextElement.FontStyleProperty, FontStyles.Italic, FontStyles.Normal);

        public void ToggleUnderline()
        {
            //if (!HasSelection) return;

            var current = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            bool isUnderlined = current != DependencyProperty.UnsetValue && current == TextDecorations.Underline;
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

        public double ChangeFontSize(double delta)
        {
           
            var current = Selection.GetPropertyValue(TextElement.FontSizeProperty);
            double currentSize = current == DependencyProperty.UnsetValue
                ? _editor.FontSize
                : Convert.ToDouble(current);
            if (_editor.Selection == null) return currentSize;

            double newSize = Math.Clamp(currentSize + delta, 8, 72);
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            return newSize;
        }

        public void Align(TextAlignment alignment)
        {
            if (_editor.Selection == null) return;
            TextPointer start = _editor.Selection.Start;
            TextPointer end = _editor.Selection.End;

            var paragraphs = _editor.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.ContentStart.CompareTo(end) < 0 &&
                            p.ContentEnd.CompareTo(start) > 0);

            foreach (Paragraph p in paragraphs)
                p.TextAlignment = alignment;
        }

        public void ApplyFontColor(Color color)
        {
            if (_editor == null) return;
            var brush = new SolidColorBrush(color);
            if (HasSelection)
                _editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            else
                _editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }
    }
}

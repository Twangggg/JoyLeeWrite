using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JoyLeeWrite.Utils
{
    public class RichTextHelper
    {
        // ✅ Flag để ngăn vòng lặp vô hạn
        private static bool _isUpdating = false;

        /// <summary>
        /// Lấy toàn bộ nội dung trong RichTextBox.
        /// </summary>
        public static string GetRichText(RichTextBox richTextBox, bool includeFormatting = false)
        {
            if (richTextBox == null || richTextBox.Document == null)
                return string.Empty;

            try
            {
                TextRange range = new TextRange(
                    richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd
                );

                if (!includeFormatting)
                {
                    return range.Text?.TrimEnd() ?? string.Empty;
                }
                else
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        range.Save(stream, DataFormats.Xaml);
                        stream.Position = 0;
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetRichText error: {ex.Message}");
                return string.Empty;
            }
        }

        public static readonly DependencyProperty BoundDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BoundDocument",
                typeof(string),
                typeof(RichTextHelper),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoundDocumentChanged
                )
            );

        public static string GetBoundDocument(DependencyObject obj)
            => (string)obj.GetValue(BoundDocumentProperty);

        public static void SetBoundDocument(DependencyObject obj, string value)
            => obj.SetValue(BoundDocumentProperty, value);

        private static void OnBoundDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox richTextBox)
                return;

            // ✅ FIX 1: Ngăn vòng lặp vô hạn
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;

                // ✅ FIX 2: Unsubscribe event trước khi modify
                richTextBox.TextChanged -= RichTextBox_TextChanged;

                // ✅ FIX 3: Sử dụng Dispatcher.Invoke để đảm bảo UI thread an toàn
                richTextBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        richTextBox.Document.Blocks.Clear();

                        if (e.NewValue is string xaml && !string.IsNullOrWhiteSpace(xaml))
                        {
                            try
                            {
                                var range = new TextRange(
                                    richTextBox.Document.ContentStart,
                                    richTextBox.Document.ContentEnd
                                );
                                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
                                range.Load(stream, DataFormats.Xaml);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Load XAML error: {ex.Message}");
                                richTextBox.Document.Blocks.Add(
                                    new Paragraph(new Run($"[Lỗi định dạng nội dung: {ex.Message}]"))
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Document update error: {ex.Message}");
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);

                // ✅ FIX 6: Re-subscribe event SAU KHI đã load xong
                richTextBox.TextChanged += RichTextBox_TextChanged;
            }
            finally
            { 
                _isUpdating = false;
            }
        }
        private static void RichTextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                richTextBox.TextChanged -= RichTextBox_TextChanged;
                richTextBox.Unloaded -= RichTextBox_Unloaded;
            }
        }

        private static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not RichTextBox richTextBox)
                return;

            // ✅ FIX 8: Ngăn vòng lặp khi đang update
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;

                // ✅ FIX 9: Lưu XAML an toàn
                var range = new TextRange(
                    richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd
                );

                using var stream = new MemoryStream();
                range.Save(stream, DataFormats.Xaml);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                var newXaml = reader.ReadToEnd();

                // ✅ FIX 10: Chỉ update nếu thực sự có thay đổi
                var currentXaml = GetBoundDocument(richTextBox);
                if (currentXaml != newXaml)
                {
                    richTextBox.SetValue(BoundDocumentProperty, newXaml);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TextChanged save error: {ex.Message}");
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
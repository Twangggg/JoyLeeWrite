using ImageMagick;
using Microsoft.Win32;
using NeoSolve.ImageSharp.AVIF;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JoyLeeWrite.Services
{
    class ImageService
    {
        public BitmapImage SelectImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Ảnh (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            };

            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage();
                using (var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            return null;
        }

        public void SaveAsAvif(BitmapImage bitmapImage, string relativePath, int? width = null, int? height = null, int quality = 50)
        {
            if (bitmapImage == null)
                throw new ArgumentNullException(nameof(bitmapImage));

            bitmapImage.Freeze();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // đi ngược lên tới thư mục chứa .csproj
            var dirInfo = new DirectoryInfo(baseDir);
            while (dirInfo != null && dirInfo.Name != "JoyLeeWrite")
            {
                dirInfo = dirInfo.Parent;
            }

            string projectDir = dirInfo?.FullName
                ?? throw new DirectoryNotFoundException("Không tìm thấy thư mục gốc project JoyLeeWrite.");

            string fullPath = Path.Combine(projectDir, relativePath);

            // Tạo thư mục nếu chưa tồn tại
            string? directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Chuyển từ BitmapImage sang MemoryStream
            using var ms = new MemoryStream();
            var encoder = new JpegBitmapEncoder
            {
                QualityLevel = 85 // giảm nhẹ
            };
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);
            ms.Position = 0;

            // Dùng Magick.NET để resize & lưu AVIF
            using (var image = new MagickImage(ms))
            {
                image.Resize(new MagickGeometry((Percentage)width.Value, (Percentage)height.Value)
                {
                    FillArea = true, // co giãn vừa đủ rồi crop phần thừa
                });
                image.Page = new MagickGeometry(0, 0, image.Width, image.Height);

                image.Format = MagickFormat.Avif;
                image.Quality = (uint)quality; // 50–80 là hợp lý
                image.Write(fullPath);
            }
        }

        public string extractPath (string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            return $"Resources/img/{title.Replace(" ", "_").ToLower()}.avif";
        }
    }
}

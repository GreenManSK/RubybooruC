using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using NLog;

namespace Rubybooru.Downloader.lib.helper
{
    public class ImageResizer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static string Resize(string path, int maxSize)
        {
            return Resize(path, maxSize, maxSize);
        }
        
        public static string Resize(string path, int maxWidth, int maxHeight)
        {
            var image = Image.FromFile(path);

            ComputeSize(image, maxWidth, maxHeight, out var width, out var height);
            
            var res = new Bitmap(width, height);  
  
            using (var graphic = Graphics.FromImage(res))  
            {  
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;  
                graphic.SmoothingMode = SmoothingMode.HighQuality;  
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;  
                graphic.CompositingQuality = CompositingQuality.HighQuality;  
                graphic.DrawImage(image, 0, 0, width, height);  
            }

            var tempFile = Path.GetTempFileName();
            res.Save(tempFile, GetFormat(path));
            return tempFile;
        }

        private static void ComputeSize(Image image, int maxWidth, int maxHeight, out int width, out int height)
        {
            if (image.Width < maxWidth && image.Height < maxHeight)
            {
                width = maxWidth;
                height = maxHeight;
                return;
            }
            if (image.Width > image.Height)
            {
                width = maxWidth;
                height = Convert.ToInt32(Math.Floor(width * (1.0 * image.Height / image.Width)));
            }
            else
            {
                height = maxHeight;
                width = Convert.ToInt32(Math.Floor(height * (1.0 * image.Width / image.Height)));
            }
        }
        
        private static ImageFormat GetFormat(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLower();
            switch (ext)
            {
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                case ".bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Jpeg;
            }
        }
    }
}
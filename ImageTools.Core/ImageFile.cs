using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ImageTools.Core
{
    public class ImageFile
    {
        public Bitmap Bitmap { get; set; }

        public string Path { get; set; }

        public ImageFile(string path, Bitmap bitmap)
        {
            Path = path;
            Bitmap = bitmap;
        }

        public static EditableImage FromFilePath(string path)
        {
            return new EditableImage(path, GetImage(path));
        }

        protected static Bitmap GetImage(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                Bitmap imgsrc = (Bitmap)Bitmap.FromStream(stream);

                return imgsrc;
            }
        }

        public Image ScaleImage(int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / Bitmap.Width;
            var ratioY = (double)maxHeight / Bitmap.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(Bitmap.Width * ratio);
            var newHeight = (int)(Bitmap.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(Bitmap, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public Image ScaleImage(int maxWidth)
        {
            var ratioX = (double)maxWidth / Bitmap.Width;

            var newWidth = (int)(Bitmap.Width * ratioX);
            var newHeight = (int)(Bitmap.Height * ratioX);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(Bitmap, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
    }
}
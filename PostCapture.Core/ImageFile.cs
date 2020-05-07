using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace PostCapture.Core
{
    public class ImageFile
    {
        public MagickImage Image { get; set; }

        public string Path { get; set; }

        public ImageFile(string path)
        {
            Path = path;

            if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
            {
                Image = new MagickImage(path, new MagickReadSettings { Format = MagickFormat.Jpg });
            }
            else
            {
                Image = new MagickImage(path);
            }
        }

        protected ImageFile(string path, MagickImage image)
        {
            Path = path;
            Image = image;
        }

        public ImageFile ScaleImage(int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / Image.Width;
            var ratioY = (double)maxHeight / Image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(Image.Width * ratio);
            var newHeight = (int)(Image.Height * ratio);

            var newImage = new MagickImage(Image);

            newImage.Resize(newWidth, newHeight);

            return new ImageFile(Path, newImage);
        }

        public ImageFile ScaleImage(int maxWidth)
        {
            var ratioX = (double)maxWidth / Image.Width;

            var newWidth = (int)(Image.Width * ratioX);
            var newHeight = (int)(Image.Height * ratioX);

            var newImage = new MagickImage(Image);

            newImage.Resize(newWidth, newHeight);

            return new ImageFile(Path, newImage);
        }
    }
}
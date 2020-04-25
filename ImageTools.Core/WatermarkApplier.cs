using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ImageTools.Core
{
    public class WatermarkApplier : IApplier
    {
        public WatermarkLocation WatermarkLocation { get; }

        public ImageFile WatermarkImage { get; }

        public WatermarkApplier(WatermarkLocation location, ImageFile watermarkImage)
        {
            WatermarkLocation = location;
            WatermarkImage = watermarkImage;
        }

        public EditableImage Apply(EditableImage image)
        {
            var bmp = image.Image;

            var shortestSide = bmp.Height > bmp.Width ? bmp.Width : bmp.Height;
            var margin = (int)Math.Round(shortestSide / 100 * WatermarkLocation.ImageMarginPercentage);
            var size = (int)Math.Round(shortestSide / 100 * WatermarkLocation.ImageSizePercentage);

            var sizedWatermark = WatermarkImage.ScaleImage(size);
            var topLeft = GetTopLeftPoint(image, sizedWatermark, WatermarkLocation.Location, margin);

            image.Image.Composite(sizedWatermark.Image, topLeft.X, topLeft.Y, CompositeOperator.Over);

            return image;
        }

        private Point GetTopLeftPoint(ImageFile mainImage, ImageFile watermark, Location location, int margin)
        {
            var x = margin;
            var y = margin;

            if(location == Location.BottomLeft)
            {
                y = mainImage.Image.Height - watermark.Image.Height - margin;
            } 
            else if(location == Location.BottomRight)
            {
                y = mainImage.Image.Height - watermark.Image.Height - margin;
                x = mainImage.Image.Width - watermark.Image.Width - margin;
            }

            return new Point(x, y);
        }
    }

    public class WatermarkLocation
    {
        public double ImageSizePercentage { get; set; }

        public double ImageMarginPercentage { get; set; }

        public Location Location { get; set; }
    }

    public enum Location
    {
        Top,
        TopLeft,
        TopRight,
        Bottom,
        BottomLeft,
        BottomRight,
        Left,
        Right
    }
}
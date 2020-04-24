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
            var bmp = image.Bitmap;

            var shortestSide = bmp.Height > bmp.Width ? bmp.Width : bmp.Height;
            var margin = (int)Math.Round(shortestSide / 100 * WatermarkLocation.ImageMarginPercentage);
            var size = (int)Math.Round(shortestSide / 100 * WatermarkLocation.ImageSizePercentage);

            var newImage = new Bitmap(bmp);

            foreach (var id in image.Bitmap.PropertyIdList)
                newImage.SetPropertyItem(image.Bitmap.GetPropertyItem(id));

            using (Graphics gfx = Graphics.FromImage(newImage))
            {
                var sizedWatermark = WatermarkImage.ScaleImage(size);
                var topLeft = GetTopLeftPoint(image.Bitmap, sizedWatermark, WatermarkLocation.Location, margin);

                gfx.DrawImage(sizedWatermark, topLeft);
            }

            image.Bitmap = newImage;

            return image;
        }

        private Point GetTopLeftPoint(Image mainImage, Image watermark, Location location, int margin)
        {
            var x = margin;
            var y = margin;

            if(location == Location.BottomLeft)
            {
                y = mainImage.Height - watermark.Height - margin;
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
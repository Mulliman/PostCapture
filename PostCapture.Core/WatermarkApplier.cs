using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace PostCapture.Core
{
    [Serializable]
    public class WatermarkApplierParams
    {
        public WatermarkLocation Location { get; set; }

        public ImageFile WatermarkImage { get; set; }
    }

    public class WatermarkApplier : IApplier
    {
        public const string IdConst = "Watermark";

        public string Id => IdConst;

        public WatermarkLocation WatermarkLocation { get; }

        public ImageFile WatermarkImage { get; }

        public WatermarkApplier(WatermarkLocation location, ImageFile watermarkImage)
        {
            WatermarkLocation = location;
            WatermarkImage = watermarkImage;
        }

        public WatermarkApplier(WatermarkApplierParams args)
        {
            WatermarkLocation = args.Location;
            WatermarkImage = args.WatermarkImage;
        }

        public WatermarkApplier(Dictionary<string, string> parameters) : this(CreateArgsFromDictionary(parameters))
        {
        }

        public EditableImage Apply(EditableImage image)
        {
            if (image == null)
            {
                return null;
            }

            if (WatermarkImage == null || WatermarkLocation == null)
            {
                return image;
            }

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

            switch (location)
            {
                case Location.BottomLeft:
                    y = mainImage.Image.Height - watermark.Image.Height - margin;
                    break;

                case Location.BottomRight:
                    y = mainImage.Image.Height - watermark.Image.Height - margin;
                    x = mainImage.Image.Width - watermark.Image.Width - margin;
                    break;

                case Location.Bottom:
                    y = mainImage.Image.Height - watermark.Image.Height - margin;
                    x = mainImage.Image.Width / 2 - watermark.Image.Width / 2;
                    break;

                case Location.Left:
                    y = mainImage.Image.Height / 2 - watermark.Image.Height / 2;
                    break;

                case Location.Right:
                    x = mainImage.Image.Width - watermark.Image.Width - margin;
                    y = mainImage.Image.Height / 2 - watermark.Image.Height / 2;
                    break;

                case Location.TopRight:
                    x = mainImage.Image.Width - watermark.Image.Width - margin;
                    break;

                case Location.Top:
                    x = mainImage.Image.Width / 2 - watermark.Image.Width / 2;
                    break;

                case Location.Middle:
                    x = mainImage.Image.Width / 2 - watermark.Image.Width / 2;
                    y = mainImage.Image.Height / 2 - watermark.Image.Height / 2;
                    break;

                case Location.TopLeft:
                default:
                    // This is the default location
                    break;
            }

            return new Point(x, y);
        }

        public static WatermarkApplierParams CreateArgsFromDictionary(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return new WatermarkApplierParams();
            }

            var watermarkPath = parameters["WatermarkImage"];

            return new WatermarkApplierParams()
            {
                Location = new WatermarkLocation
                {
                    ImageMarginPercentage = double.Parse(parameters["ImageMarginPercentage"]),
                    ImageSizePercentage = double.Parse(parameters["ImageSizePercentage"]),
                    Location = Enum.Parse<Location>(parameters["Location"]),
                },
                WatermarkImage = new ImageFile(watermarkPath)
            };
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
        Middle,
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
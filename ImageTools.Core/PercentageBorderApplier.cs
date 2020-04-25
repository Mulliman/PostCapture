using ImageMagick;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageTools.Core
{
    public class PercentageBorderApplier : IApplier
    {
        public double BorderWidthPercentage { get; }

        public Color Colour { get; }

        public PercentageBorderApplier(double percentageWidth, Color colour)
        {
            BorderWidthPercentage = percentageWidth;
            Colour = colour;
        }

        public EditableImage Apply(EditableImage image)
        {
            var bmp = image.Image;

            var borderWidth = (int)Math.Round(bmp.Width / 100 * BorderWidthPercentage);

            // For thicker borders try and keep to jpeg size blocks.
            if (borderWidth > 16)
            {
                borderWidth = (int)Math.Round(borderWidth / 8d) * 8;
            }

            int newWidth = bmp.Width + (borderWidth * 2);
            int newHeight = bmp.Height + (borderWidth * 2);

            var newImage = new MagickImage(MagickColor.FromRgb(Colour.R, Colour.G, Colour.B), newWidth, newHeight);

            using (var clonedOriginal = image.Image.Clone())
            {
                newImage.Composite(clonedOriginal, Gravity.Center);

                image.Image = newImage;

                return image;
            }
        }
    }
}
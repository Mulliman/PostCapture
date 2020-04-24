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
            var bmp = image.Bitmap;

            var borderWidth = (int)Math.Round(bmp.Width / 100 * BorderWidthPercentage);

            // For thicker borders try and keep to jpeg size blocks.
            if(borderWidth > 16)
            {
                borderWidth = (int)Math.Round(borderWidth / 8d) * 8;
            }

            int newWidth = bmp.Width + (borderWidth * 2);
            int newHeight = bmp.Height + (borderWidth * 2);

            var newImage = new Bitmap(newWidth, newHeight);

            foreach (var id in image.Bitmap.PropertyIdList)
                newImage.SetPropertyItem(image.Bitmap.GetPropertyItem(id));

            using (Graphics gfx = Graphics.FromImage(newImage))
            {
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (Brush border = new SolidBrush(Colour))
                {
                    gfx.FillRectangle(border, 0, 0, newWidth, newHeight);
                }

                gfx.DrawImage(bmp, new Rectangle(borderWidth, borderWidth, bmp.Width, bmp.Height));
            }

            image.Bitmap = newImage;

            return image;
        }
    }
}
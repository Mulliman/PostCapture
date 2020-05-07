using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PostCapture.Core
{
    [Serializable]
    public class PercentageBorderApplierParams
    {
        public double BorderWidthPercentage { get; set; }

        public int R { get; set; }

        public int G { get; set; }

        public int B { get; set; }
    }

    public class PercentageBorderApplier : IApplier
    {
        public const string IdConst = "PercentageBorder";

        public double BorderWidthPercentage { get; }

        public Color Colour { get; }

        public string Id => IdConst;

        public PercentageBorderApplier(double percentageWidth, Color colour)
        {
            BorderWidthPercentage = percentageWidth;
            Colour = colour;
        }

        public PercentageBorderApplier(PercentageBorderApplierParams args)
        {
            BorderWidthPercentage = args.BorderWidthPercentage;
            Colour = Color.FromArgb(args.R, args.G, args.B);
        }

        public PercentageBorderApplier(Dictionary<string, string> parameters) : this(CreateArgsFromDictionary(parameters))
        {
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

            int newWidth = bmp.Width + borderWidth * 2;
            int newHeight = bmp.Height + borderWidth * 2;

            var newImage = new MagickImage(MagickColor.FromRgb(Colour.R, Colour.G, Colour.B), newWidth, newHeight);

            using (var clonedOriginal = image.Image.Clone())
            {
                newImage.SetProfile(clonedOriginal.GetExifProfile());
                newImage.Composite(clonedOriginal, Gravity.Center);

                image.Image = newImage;

                return image;
            }
        }

        public static PercentageBorderApplierParams CreateArgsFromDictionary(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return new PercentageBorderApplierParams();
            }

            return new PercentageBorderApplierParams()
            {
                BorderWidthPercentage = double.Parse(parameters["BorderWidthPercentage"]),
                R = int.Parse(parameters["R"]),
                G = int.Parse(parameters["G"]),
                B = int.Parse(parameters["B"])
            };
        }
    }
}
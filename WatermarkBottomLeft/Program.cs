using ImageTools.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace WatermarkBottomLeft
{
    public class Program
    {
        private static readonly Dictionary<Color, double> _borders = new Dictionary<Color, double>()
        {
            { Color.White, 0.25 },
            { Color.Black, 2 }
        };

        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            ImagePath = args[0];

            var editableImage = EditableImage.FromFilePath(ImagePath);

            var watermarkLocation = new WatermarkLocation
            {
                Location = Location.BottomLeft,
                ImageMarginPercentage = 2.5,
                ImageSizePercentage = 7
            };

            var watermarkImage = EditableImage.FromFilePath(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "watermark.png"));

            var watermarkApplier = new WatermarkApplier(watermarkLocation, watermarkImage);
            watermarkApplier.Apply(editableImage);

            foreach (var border in _borders)
            {
                var borderApplier = new PercentageBorderApplier(border.Value, border.Key);
                editableImage = borderApplier.Apply(editableImage);
            }

            editableImage.Save();
        }
    }
}

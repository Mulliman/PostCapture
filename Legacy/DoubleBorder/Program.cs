using PostCapture.Core;
using System.Collections.Generic;
using System.Drawing;

namespace Border
{
    public class Program
    {
        private static readonly Dictionary<Color, double> _borders = new Dictionary<Color, double>()
        {
            { Color.White, 0.25 },
            { Color.Black, 3 }
        };

        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            ImagePath = args[0];

            var editableImage = EditableImage.FromFilePath(ImagePath);

            foreach(var border in _borders)
            {
                var borderApplier = new PercentageBorderApplier(border.Value, border.Key);
                editableImage = borderApplier.Apply(editableImage);
            }

            editableImage.Save();
        }
    }
}
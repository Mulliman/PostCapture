using System.Drawing;
using PostCapture.Core;

namespace Border
{
    public class Program
    {
        const int BorderWidthPercentage = 3;
        static readonly Color BorderColor = Color.Black;

        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            ImagePath = args[0];

            var editableImage = EditableImage.FromFilePath(ImagePath);

            var borderApplier = new PercentageBorderApplier(BorderWidthPercentage, BorderColor);

            editableImage = borderApplier.Apply(editableImage);

            editableImage.Save();
        }
    }
}
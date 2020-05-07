using PostCapture.Core;
using PostCapture.Core.Selection;

namespace PostCapture.Process
{
    class Program
    {
        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            ImagePath = args[0];

            var editableImage = EditableImage.FromFilePath(ImagePath);

            var selector = new MetadataSelector();

            var steps = selector.GetProcessSteps(editableImage);

            foreach (var step in steps.Steps)
            {
                editableImage = step.Apply(editableImage);
            }

            editableImage.Save();
        }
    }
}
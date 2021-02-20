using PostCapture.Core;
using PostCapture.Core.Selection;
using System;

namespace PostCapture.Process
{
    class Program
    {
        private static bool _isDebugging = false;

        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            try
            {
                ImagePath = args[0];

                if(_isDebugging)
                {
                    Console.WriteLine("Press key to start");
                    Console.ReadLine();
                    Console.WriteLine(ImagePath);
                }

                var editableImage = EditableImage.FromFilePath(ImagePath);

                if (_isDebugging)
                {
                    Console.WriteLine(editableImage.Path);
                }

                var selector = new MetadataSelector();

                var steps = selector.GetProcessSteps(editableImage);

                if (_isDebugging)
                {
                    foreach(var step in steps.Steps)
                    {
                        Console.WriteLine(step.Id);
                    }
                }

                foreach (var step in steps.Steps)
                {
                    editableImage = step.Apply(editableImage);
                }

                if (_isDebugging)
                {
                    Console.WriteLine("Ready to write");
                }

                editableImage.Save();

                if (_isDebugging)
                {
                    Console.WriteLine("END");
                    Console.ReadLine();
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Please press enter to close this once you have addressed the error.");
                Console.ReadLine();
            }
        }
    }
}
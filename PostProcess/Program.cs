using ImageTools.Core;
using ImageTools.Core.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace PostProcess
{
    class Program
    {
        public static string ImagePath { get; private set; }

        static void Main(string[] args)
        {
            ImagePath = args[0];

            var editableImage = EditableImage.FromFilePath(ImagePath);

            var selector = new ExifBasedSelector();

            var steps = selector.GetProcessSteps(editableImage);

            foreach(var step in steps.Steps)
            {
                editableImage = step.Apply(editableImage);
            }

            editableImage.Save();
        }
    }
}
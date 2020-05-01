using ImageTools.Core.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageTools.Core.Selection
{
    public class ExifBasedSelector
    {
        private List<ProcessConfigurationFile> _configs;

        public ExifBasedSelector()
        {
            var folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");
            var repo = new ProcessStepRepository(folder);
            _configs = repo.Configs;
        }

        public ProcessSteps GetProcessSteps(ImageFile imageFile)
        {
            string processId = GetProcessId(imageFile);

            var config = _configs.FirstOrDefault(c => c.Id.ToLower() == processId.ToLower());

            var list = new List<IApplier>();
            var builder = new ApplierBuilder();

            foreach(var step in config.Steps)
            {
                list.Add(builder.CreateApplier(step));
            }

            return new ProcessSteps(list);
        }

        private string GetProcessId(ImageFile imageFile)
        {
            var profile = imageFile.Image.GetExifProfile();

            var make = profile.Values.FirstOrDefault(v => "make".Equals(v.Tag.ToString(), StringComparison.OrdinalIgnoreCase));

            foreach (var value in profile.Values)
            {
                Console.WriteLine("{0}({1}): {2}", value.Tag, value.DataType, value.ToString());
            }

            return make.GetValue().ToString();
        }
    }
}
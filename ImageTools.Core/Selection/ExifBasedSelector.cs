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
        public string Folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");

        public ExifBasedSelector()
        {
            var repo = new ProcessStepRepository(Folder);
            _configs = repo.Configs;
        }

        public ProcessSteps GetProcessSteps(ImageFile imageFile)
        {
            string processId = GetProcessId(imageFile);

            if(processId == null)
            {
                return new ProcessSteps(new List<IApplier>());
            }

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

            foreach(var config in _configs)
            {
                var matchCategory = config.MatchProperty;
                var matchValue = config.MatchValue;

                var imageCategory = profile.Values.FirstOrDefault(v => matchCategory.Equals(v.Tag?.ToString(), StringComparison.OrdinalIgnoreCase));
                if(string.Equals(imageCategory?.GetValue()?.ToString(), matchValue, StringComparison.OrdinalIgnoreCase))
                {
                    return config.Id;
                }
            }

            foreach (var value in profile.Values)
            {
                Console.WriteLine("{0}({1}): {2}", value.Tag, value.DataType, value.ToString());
            }

            return null;
        }
    }
}
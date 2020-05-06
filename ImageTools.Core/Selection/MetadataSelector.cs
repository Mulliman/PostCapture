using ImageMagick;
using ImageTools.Core.Builder;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Xmp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageTools.Core.Selection
{
    public class MetadataSelector
    {
        private List<ProcessConfigurationFile> _configs;
        public string Folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");

        public MetadataSelector()
        {
            var repo = new ProcessStepRepository(Folder);
            _configs = repo.Configs.OrderBy(c => c.Priority).ToList();
        }

        public ProcessSteps GetProcessSteps(ImageFile imageFile)
        {
            string processId = GetProcessId(imageFile);

            if (processId == null)
            {
                return new ProcessSteps(new List<IApplier>());
            }

            var config = _configs.FirstOrDefault(c => c.Id.ToLower() == processId.ToLower());

            var list = new List<IApplier>();
            var builder = new ApplierBuilder();

            foreach (var step in config.Steps)
            {
                list.Add(builder.CreateApplier(step));
            }

            return new ProcessSteps(list);
        }

        private string GetProcessId(ImageFile imageFile)
        {
            var availableCategoriesAndValues = GetLoweredCategoryValueDictionary(imageFile);

            foreach (var config in _configs)
            {
                var matchCategory = config.MatchProperty?.ToLower();
                var matchValue = config.MatchValue?.ToLower();

                if(availableCategoriesAndValues.ContainsKey(matchCategory))
                {
                    var value = availableCategoriesAndValues[matchCategory];

                    if(value == matchValue)
                    {
                        return config.Id;
                    }
                }
            }

            return null;
        }

        public static Dictionary<string, string> GetCategoryValueDictionary(ImageFile imageFile)
        {
            var dict = new Dictionary<string,string>();
       
            var metadata = ImageMetadataReader.ReadMetadata(imageFile.Path);
            
            foreach(var type in metadata)
            {
                foreach (var tag in type.Tags)
                {
                    if (!dict.ContainsKey(tag.Name))
                    {
                        dict.Add(tag.Name, tag.Description);
                    }
                }
            }

            // XMP needs special treatment
            var xmpDirectory = metadata.OfType<XmpDirectory>().FirstOrDefault();
            if (xmpDirectory != null)
            {
                foreach (var value in xmpDirectory.GetXmpProperties())
                {
                    if (!dict.ContainsKey(value.Key))
                    {
                        dict.Add(value.Key, value.Value);
                    }
                }
            }

            return dict;
        }

        public static Dictionary<string, string> GetLoweredCategoryValueDictionary(ImageFile imageFile)
        {
            return GetCategoryValueDictionary(imageFile)
                .ToDictionary(d => d.Key?.ToLower(), d => d.Value?.ToLower());
        }
    }
}
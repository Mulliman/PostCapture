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
    public class ExifBasedSelector
    {
        private List<ProcessConfigurationFile> _configs;
        public string Folder = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "processes");

        public ExifBasedSelector()
        {
            var repo = new ProcessStepRepository(Folder);
            _configs = repo.Configs.OrderBy(c => c.Priority).ToList();
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
            var profile2 = imageFile.Image.GetIptcProfile();

            foreach (var config in _configs)
            {
                var matchCategory = config.MatchProperty;
                var matchValue = config.MatchValue;

                var imageCategory = profile.Values.FirstOrDefault(v => matchCategory.Equals(v.Tag?.ToString(), StringComparison.OrdinalIgnoreCase));
                var imageCategoryValue = imageCategory?.GetValue()?.ToString();

                foreach (var value in profile.Values)
                {
                    Console.WriteLine("{0}({1}): {2}", value.Tag, value.DataType, value.ToString());
                }

                if (imageCategoryValue == null)
                {
                    // try to get from IPTC profile instead.
                    var iptcCategory = profile2.Values.FirstOrDefault(v => matchCategory.Equals(Enum.GetName(typeof(IptcTag), v.Tag), StringComparison.OrdinalIgnoreCase));
                    imageCategoryValue = iptcCategory?.Value?.ToString();

                    foreach (var value in profile2.Values)
                    {
                        Console.WriteLine("{0}({1}, {2}): {3}", value.Tag, Enum.GetName(typeof(IptcTag), value.Tag), value.Value, value.ToString());
                    }

                    if(imageCategoryValue == null)
                    {
                        var lowerMatchKey = matchCategory.ToLower();

                        var metadata = ImageMetadataReader.ReadMetadata(imageFile.Path);
                        var xmpDirectory = metadata.OfType<XmpDirectory>().FirstOrDefault();
                        var xmpDictionary = xmpDirectory.GetXmpProperties().ToDictionary(x => x.Key.ToLower(), x => x.Value);

                        if (imageCategoryValue == null && xmpDictionary.ContainsKey(lowerMatchKey))
                        {
                            // try to get from XMP profile instead.
                            imageCategoryValue = xmpDictionary[lowerMatchKey];
                        }

                        foreach (var value in profile2.Values)
                        {
                            Console.WriteLine("{0}({1}, {2}): {3}", value.Tag, Enum.GetName(typeof(IptcTag), value.Tag), value.Value, value.ToString());
                        }
                    }
                }

                if (string.Equals(imageCategoryValue, matchValue, StringComparison.OrdinalIgnoreCase))
                {
                    return config.Id;
                }
            }

            return null;
        }
    }
}
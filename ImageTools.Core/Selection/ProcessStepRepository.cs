using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageTools.Core.Selection
{
    public class ProcessStepRepository
    {
        public List<ProcessConfigurationFile> Configs { get; }

        public ProcessStepRepository(string configsFolderPath)
        {
            string[] files = Directory.GetFiles(configsFolderPath);

            var configs = new List<ProcessConfigurationFile>();

            foreach (string fileName in files.Where(f => f.EndsWith(".json")))
            {
                var text = File.ReadAllText(fileName);
                var config = JsonConvert.DeserializeObject<ProcessConfigurationFile>(text);
                config.Filename = fileName;
                configs.Add(config);
            }

            Configs = configs;
        }

        public void SaveProcessConfigurationFile(ProcessConfigurationFile file)
        {
            var text = JsonConvert.SerializeObject(file, Formatting.Indented);

            File.WriteAllText(file.Filename, text);
        }
    }

    public class ProcessConfigurationFile : ProcessConfiguration
    {
        public string Filename { get; set; }
    }

    public class ProcessConfiguration
    {
        public string Id { get; set; }

        public string MatchProperty { get; set; }

        public string MatchValue { get; set; }

        public List<ProcessStepConfiguration> Steps { get; set; }
    }

    public class ProcessStepConfiguration
    {
        public string Id { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
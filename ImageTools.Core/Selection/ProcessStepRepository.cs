using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageTools.Core.Selection
{
    public class ProcessStepRepository
    {
        public List<ProcessConfiguration> Configs { get; }

        public ProcessStepRepository(string configsFolderPath)
        {
            string[] files = Directory.GetFiles(configsFolderPath);

            var configs = new List<ProcessConfiguration>();

            foreach (string fileName in files.Where(f => f.EndsWith(".json")))
            {
                var text = File.ReadAllText(fileName);
                var config = JsonConvert.DeserializeObject<ProcessConfiguration>(text);
                configs.Add(config);
            }

            Configs = configs;
        }
    }

    public class ProcessConfiguration
    {
        public string Id { get; set; }

        public List<ProcessStepConfiguration> Steps { get; set; }
    }

    public class ProcessStepConfiguration
    {
        public string Id { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
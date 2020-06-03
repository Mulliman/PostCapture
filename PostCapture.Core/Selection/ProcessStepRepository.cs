using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PostCapture.Core.Selection
{
    public class ProcessStepRepository
    {
        private readonly string _configsFolderPath;

        public List<ProcessConfigurationFile> Configs { get; }

        public ProcessStepRepository(string configsFolderPath)
        {
            _configsFolderPath = configsFolderPath;

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

        public string GetSafeFilename(string name)
        {
            var firstAttempt = Path.Combine(_configsFolderPath, name + ".json");

            if (!File.Exists(firstAttempt))
            {
                return firstAttempt;
            }

            return Path.Combine(_configsFolderPath, name + "-" + string.Concat(Guid.NewGuid().ToString("N").Take(5)) + ".json");
        }

        public bool IsSafeId(string name)
        {
            return Configs.All(c => c.Id.ToLower() != name.ToLower());
        }

        public void DeleteProcessConfigurationFile(ProcessConfigurationFile file)
        {
            if (!File.Exists(file.Filename))
            {
                throw new Exception($"File {file.Filename} does not exist.");
            }

            File.Delete(file.Filename);

            var index = Configs.FindIndex(c => c.Id.Equals(file.Id, StringComparison.OrdinalIgnoreCase));
            Configs.RemoveAt(index);
        }

        public void SaveProcessConfigurationFile(ProcessConfigurationFile file)
        {
            var text = JsonConvert.SerializeObject(file, Formatting.Indented);

            if (!File.Exists(file.Filename))
            {
                throw new Exception($"File {file.Filename} does not exist.");
            }

            File.WriteAllText(file.Filename, text);

            // Replace config in list with updated one.
            Configs[Configs.FindIndex(c => c.Id.Equals(file.Id, StringComparison.OrdinalIgnoreCase))] = file;
        }

        public void CreateProcessConfigurationFile(ProcessConfigurationFile file)
        {
            var text = JsonConvert.SerializeObject(file, Formatting.Indented);

            File.WriteAllText(file.Filename, text);

            Configs.Add(file);
        }
    }

    public class ProcessConfigurationFile : ProcessConfiguration
    {
        public string Filename { get; set; }

        public string NameAndPriority => $"({Priority}) {Id}";
    }

    public class ProcessConfiguration
    {
        public string Id { get; set; }

        public string MatchProperty { get; set; }

        public string MatchValue { get; set; }

        public List<ExtraMatchCriterion> ExtraMatchCriteria { get; set; }

        public int Priority { get; set; }

        public List<ProcessStepConfiguration> Steps { get; set; }
    }

    public class ExtraMatchCriterion
    {
        public string MatchProperty { get; set; }

        public string MatchValue { get; set; }
    }

    public class ProcessStepConfiguration
    {
        public string Id { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
}
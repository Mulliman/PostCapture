using System;
using System.IO;

namespace PostCapture.Core.StorageLocations
{
    public class AppDataStorageLocation
    {
        public string BasePath { get; }
        public string ProcessesPath { get; } 

        public AppDataStorageLocation()
        {
            BasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostCapture");
            ProcessesPath = System.IO.Path.Combine(BasePath, "Processes");

            // Ensure Created
            if(!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }

            if (!Directory.Exists(ProcessesPath))
            {
                Directory.CreateDirectory(ProcessesPath);
            }
        }
    }
}
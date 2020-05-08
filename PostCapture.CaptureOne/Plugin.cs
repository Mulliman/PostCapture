﻿using PhaseOne.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace PostCapture.CaptureOne
{
    public class Plugin : ISettingsPlugin, IOpenWithPlugin, IEditingPlugin
    {
        private readonly string _rootDirectory;

        public string ManagementProgramPath { get; set; }

        public string PostProcessProgramPath { get; set; }

        private static readonly BitmapSource Icon = GetImage(Environment.CurrentDirectory + "/miniIcon.png");

        private readonly PluginAction _openConfiguratorAction = new PluginAction(
                    "Set up PostCapture process",
                    "OpenConfigurator")
        {
            Image = Icon
        };

        private readonly PluginAction _runProcessAction = new PluginAction(
                    "Apply PostCapture process",
                    "ApplyProcess")
        {
            Image = Icon
        };

        public Plugin()
        {
            _rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ManagementProgramPath = Path.Combine(_rootDirectory, "PostCapture.Studio.exe");
            PostProcessProgramPath = Path.Combine(_rootDirectory, "PostCapture.Process.exe");
        }

        #region ISettingsPlugin

        public IEnumerable<ElementsGroup> GetSettings()
        {
            var items = new List<Element>();

            var openSettingsButton = new ButtonItem("openSettings", "Open Configurator");
            items.Add(openSettingsButton);

            var pathToProcessExe = new TextItem("pathToProcessExe", "Post Process Program Path")
            {
                Value = PostProcessProgramPath,
                InformativeText = "Copy and paste this path into process recipes so that it runs once Capture One has finished processing the image."
            };
            items.Add(pathToProcessExe);

            var settings = new ElementsGroup("settings", "", items.ToArray());

            return new[]
            {
                settings,
            };
        }

        public bool UpdateSettings(string argKey, object argValue)
        {
            // don't refresh the settings
            return false;
        }

        public bool HandleEvent(SettingsEvent argSettingsEvent, Item argItem)
        {
            if(argItem.Id == "openSettings")
            {
                Process.Start(ManagementProgramPath);
                return true;
            }

            return false;
        }

        #endregion

        #region Open With and Edit With

        public IEnumerable<PluginAction> GetOpenWithActions(IDictionary<string, int> argInfo, OpenWithPluginRole argRole)
        {
            if(argRole == OpenWithPluginRole.OpenWithPluginRolePostProcessInDocument)
            {
                return Enumerable.Empty<PluginAction>();
            }

            if (argRole == OpenWithPluginRole.OpenWithPluginRolePostProcessOutput)
            {
                return new[] { _runProcessAction };
            }

            if (!argInfo.Any(a => a.Key == ".jpg" || a.Key == ".jpeg"))
            {
                return Enumerable.Empty<PluginAction>();
            }

            return new[] { _openConfiguratorAction, _runProcessAction };
        }

        public IEnumerable<PluginAction> GetEditingActions(IDictionary<string, int> argInfo)
        {
            return new[] { _openConfiguratorAction, _runProcessAction };
        }

        public PluginActionOpenWithResult StartOpenWithTask(FileHandlingPluginTask argTask, ReportProgress argProgress)
        {
            if(argTask.PluginAction.Identifier == _openConfiguratorAction.Identifier)
            {
                StartPostCaptureStudio(argTask);
            }

            if(argTask.PluginAction.Identifier == _runProcessAction.Identifier)
            {
                StartPostCaptureProcessor(argTask);
            }

            return new PluginActionOpenWithResult();
        }

        public PluginActionImageResult StartEditingTask(FileHandlingPluginTask argPluginTask, ReportProgress argProgress)
        {
            if (argPluginTask.PluginAction.Identifier == _openConfiguratorAction.Identifier)
            {
                StartPostCaptureStudio(argPluginTask);
            }

            if (argPluginTask.PluginAction.Identifier == _runProcessAction.Identifier)
            {
                StartPostCaptureProcessor(argPluginTask);
            }

            return new PluginActionImageResult();
        }

        public IEnumerable<FileHandlingPluginTask> GetTasks(PluginAction argPluginAction, IEnumerable<string> argFiles)
        {
            // one task for all files
            if (argPluginAction.Identifier == _openConfiguratorAction.Identifier)
            {
                var firstFile = argFiles.FirstOrDefault(f => f != null && (f.EndsWith("jpeg") || f.EndsWith(".jpg")));

                if (firstFile != null)
                {
                    return new[]
                    {
                        new FileHandlingPluginTask(Guid.NewGuid(), argPluginAction, new [] { firstFile }),
                    };
                }
            }
            else if (argPluginAction.Identifier == _runProcessAction.Identifier)
            {
                var files = argFiles.Where(f => f != null && (f.EndsWith("jpeg") || f.EndsWith(".jpg")));

                if (files.Any())
                {
                    return new[]
                    {
                        new FileHandlingPluginTask(Guid.NewGuid(), argPluginAction, files.ToArray()),
                    };
                }
            }

            var tasks = argFiles.Select(f => new FileHandlingPluginTask(Guid.NewGuid(), argPluginAction, new[] { f }));
            return tasks;
        }

        #endregion

        private void StartPostCaptureProcessor(FileHandlingPluginTask argTask)
        {
            var files = argTask.Files.Where(f => f != null && (f.EndsWith(".jpeg") || f.EndsWith(".jpg")));

            if (!files.Any())
            {
                return;
            }

            foreach (var file in files)
            {
                Process.Start(PostProcessProgramPath, "\"" + file + "\"");
            }
        }

        private void StartPostCaptureStudio(FileHandlingPluginTask argTask)
        {
            var firstFile = argTask.Files.FirstOrDefault(f => f != null && (f.EndsWith(".jpeg") || f.EndsWith(".jpg")));

            if (firstFile == null)
            {
                return;
            }

            Process.Start(ManagementProgramPath, "\"" + firstFile + "\"");
        }

        private static BitmapImage GetImage(string argPath)
        {
            Stream stream = File.Open(argPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            BitmapImage imgsrc = new BitmapImage();

            imgsrc.BeginInit();
            imgsrc.StreamSource = stream;
            imgsrc.EndInit();

            return imgsrc;
        }
    }
}
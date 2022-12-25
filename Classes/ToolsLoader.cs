using Paint.Classes.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Paint.Classes
{
    public class ToolsLoader
    {
        public class LoadedToolData
        {
            private List<Tool> items = new List<Tool>();
            private string file;

            public LoadedToolData(List<Tool> instruments, string filePath)
            {
                items = instruments;
                file = filePath;
            }

            public List<Tool> Items => items;
            public string File => file;
        }

        const string PluginTypeName = "Paint.Classes.Instruments.IPlugin";
        List<Tool> instrumentsList = new List<Tool>();

        List<LoadedToolData> loadedInstruments = new List<LoadedToolData>();

        string[] paths = new string[0];

        public string Folder { get; private set; }

        public void FindFiles(LocalFileSystem fileSystem)
        {
            paths = Directory.GetFiles(fileSystem.Root + fileSystem.AddonsToolsPath, "*.dll");
        }

        public ToolsLoader(LocalFileSystem fileSystem, MainWindow mainWindow)
        {
            Folder = fileSystem.Root + fileSystem.AddonsPath;
            FindFiles(fileSystem);
            if (paths.Length == 0)
            {
                var standardBrushes = System.AppDomain.CurrentDomain.BaseDirectory + "/Bin/StandardTools.dll";
                if (File.Exists(standardBrushes))
                {
                    File.Copy(standardBrushes, fileSystem.Root + fileSystem.AddonsToolsPath + "/StandardTools.dll");
                    FindFiles(fileSystem);
                }
                else
                {
                    MessageBox.Show("Can't find .dll file of Standard Tools!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    mainWindow.Close();
                    return;
                }
            }

            for (int i = 0; i < paths.Length; i++)
            {
                var items = LoadPluginsFromFile(paths[i]);
                List<Tool> loaded = new List<Tool>();
                foreach (var plugin in items)
                {
                    var instrument = ((Tool)plugin);
                    instrument.Init(mainWindow);
                    instrumentsList.Add(instrument);
                    loaded.Add(instrument);
                }
                if (loaded.Count != 0)
                    loadedInstruments.Add(new LoadedToolData(loaded, paths[i]));
            }
        }

        public List<Tool> GetInstruments() => instrumentsList;
        public List<LoadedToolData> LoadedData() => loadedInstruments;

        private List<IPlugin> LoadPluginsFromFile(string fileName)
        {
            Assembly asm;
            IPlugin plugin;
            List<IPlugin> plugins;
            Type tInterface;

            plugins = new List<IPlugin>();
            asm = Assembly.LoadFrom(fileName);

            foreach (Type t in asm.GetTypes())
            {
                tInterface = t.GetInterface(PluginTypeName);
                if (tInterface != null)
                {
                    plugin = (IPlugin)Activator.CreateInstance(t);
                    plugins.Add(plugin);
                }
            }
            return plugins;
        }
    }
}

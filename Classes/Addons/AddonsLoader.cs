using Paint.Classes.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Paint.Classes
{
    public partial class AddonsLoader
    {
        LoadedDataCollector<Tool> tools = new LoadedDataCollector<Tool>();
        LoadedDataCollector<Plugin> plugins = new LoadedDataCollector<Plugin>();


        List<string> paths = new List<string>();

        public string Folder { get; private set; }

        public void FindFiles(LocalFileSystem fileSystem)
        {
            paths.Clear();
            paths.AddRange(Directory.GetFiles(fileSystem.Root + fileSystem.AddonsToolsPath, "*.dll").ToList());
            paths.AddRange(Directory.GetFiles(fileSystem.Root + fileSystem.AddonsPluginsPath, "*.dll").ToList());
        }

        public AddonsLoader(LocalFileSystem fileSystem, MainWindow mainWindow)
        {
            Folder = fileSystem.Root + fileSystem.AddonsPath;
            FindFiles(fileSystem);
            CheckFilesValid(fileSystem, mainWindow);
            LoadFiles(mainWindow);

        }



        const string PluginTypeName = "Paint.Classes.Instruments.ILoadable";
        private List<ILoadable> LoadAddonsFromFile(string fileName)
        {
            Assembly asm;
            ILoadable plugin;
            List<ILoadable> plugins;
            Type tInterface;

            plugins = new List<ILoadable>();
            asm = Assembly.LoadFrom(fileName);

            foreach (Type t in asm.GetTypes())
            {
                tInterface = t.GetInterface(PluginTypeName);
                if (tInterface != null)
                {
                    plugin = (ILoadable)Activator.CreateInstance(t);
                    plugins.Add(plugin);
                }
            }
            return plugins;
        }
        public void LoadFiles(MainWindow mainWindow)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                var items = LoadAddonsFromFile(paths[i]);

                List<Tool> tools = new List<Tool>();
                List<Plugin> plugins = new List<Plugin>();

                foreach (var loadedClass in items)
                {
                    try
                    {
                        if (loadedClass is IPlugin)
                        {
                            var plugin = (Plugin)loadedClass;
                            plugin.Init(mainWindow);
                            plugins.Add(plugin);
                        }
                        if (loadedClass is ITool)
                        {
                            var tool = (Tool)loadedClass;
                            tool.Init(mainWindow);
                            tools.Add(tool);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error", "Load File Error: " + paths[i], MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                }

                if (tools.Count != 0)
                {
                    this.tools.AddItem(tools, paths[i]);
                }
                if (plugins.Count != 0)
                {
                    this.plugins.AddItem(plugins, paths[i]);
                }
            }
        }




        public void CheckFilesValid(LocalFileSystem fileSystem, MainWindow mainWindow)
        {
            if (Directory.Exists(fileSystem.Root + "/Bin/Assets"))
            {
                Directory.Delete(fileSystem.Root + "/Bin/Assets", true);
            }

            if (paths.Count == 0)
            {
                CopyDLL("StandardPlugins.dll", fileSystem.AddonsPluginsPath, "Standard Plugins");
                CopyDLL("StandardTools.dll", fileSystem.AddonsToolsPath, "Standard Plugins");   
            }

            void CopyDLL(string dllPath,string systemPath, string itemName)
            {
                var file = fileSystem.Bin + dllPath;
                if (File.Exists(file))
                {
                    File.Copy(file, fileSystem.Root + systemPath + $"/{Path.GetFileNameWithoutExtension(dllPath)}.dll");
                    FindFiles(fileSystem);
                }
                else
                {
                    MessageBox.Show($"Can't find .dll file of {itemName}!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    mainWindow.Close();
                    return;
                }
            }
        }

        public LoadedDataCollector<Tool> GetTools() => tools;
        public LoadedDataCollector<Plugin> GetPlugins() => plugins;
        public void PluginsAfterWindowLoaded()
        {
            foreach (var item in plugins.AllItems)
            {
                item.AfterWindowLoaded();
            }
        }
    }
}

using Paint.Classes.Instruments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Paint.Classes
{
    public class InstrumentsLoader
    {
        public class LoadedInstrumentData
        {
            private List<Instrument> items = new List<Instrument>();
            private string file;

            public LoadedInstrumentData(List<Instrument> instruments, string filePath)
            {
                items = instruments;
                file = filePath;
            }

            public List<Instrument> Items => items;
            public string File => file;
        }

        const string PluginTypeName = "Paint.Classes.Instruments.IPlugin";
        List<Instrument> instrumentsList = new List<Instrument>();

        List<LoadedInstrumentData> loadedInstruments = new List<LoadedInstrumentData>();

        string[] paths = new string[0];

        public string Folder { get; private set; }

        public void FindFiles(LocalFileSystem fileSystem)
        {
            paths = Directory.GetFiles(fileSystem.Root + fileSystem.AddonsToolsPath, "*.dll");
        }

        public InstrumentsLoader(LocalFileSystem fileSystem, MainWindow mainWindow)
        {
            Folder = fileSystem.Root + fileSystem.AddonsPath;
            FindFiles(fileSystem);
            if (paths.Length == 0)
            {
                var standardBrushes = System.AppDomain.CurrentDomain.BaseDirectory + "/Bin/StartBrushes.dll";
                if (File.Exists(standardBrushes))
                {
                    File.Copy(standardBrushes, fileSystem.Root + fileSystem.AddonsToolsPath + "/StartBrushes.dll");
                    FindFiles(fileSystem);
                }
                else
                {
                    MessageBox.Show("Cant find standard brushes!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    mainWindow.Close();
                    return;
                }
            }

            for (int i = 0; i < paths.Length; i++)
            {
                var items = LoadPluginsFromFile(paths[i]);
                List<Instrument> loaded = new List<Instrument>();
                foreach (var plugin in items)
                {
                    var instrument = ((Instrument)plugin);
                    instrument.Init(mainWindow);
                    instrumentsList.Add(instrument);
                    loaded.Add(instrument);
                }
                if (loaded.Count != 0)
                    loadedInstruments.Add(new LoadedInstrumentData(loaded, paths[i]));
            }
        }

        public List<Instrument> GetInstruments() => instrumentsList;
        public List<LoadedInstrumentData> LoadedData() => loadedInstruments;

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

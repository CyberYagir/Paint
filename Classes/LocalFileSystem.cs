using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Paint.Classes
{
    public sealed class LocalFileSystem
    {
        public string Bin { get; private set; }

        public string Root { get; private set; }
        public string FileSystemRoot { get; private set; }
        public string BrushesPath { get; private set; }
        private string LocalResources { get; set; }
        public string AddonsPath { get; set; }
        public string AddonsToolsPath { get; set; }
        public string AddonsPluginsPath { get; set; }

        public Dictionary<string, string> Resources { get; private set; } = new Dictionary<string, string>();

        public LocalFileSystem()
        {
            Bin = AppDomain.CurrentDomain.BaseDirectory + "/Bin/";
            Root = AppDomain.CurrentDomain.BaseDirectory;
            Root = Directory.GetParent(Root).FullName;
            Root = Directory.GetParent(Root).FullName;


            FileSystemRoot = "/Assets/";
            BrushesPath = FileSystemRoot + "/Brushes";
            LocalResources = FileSystemRoot + "/LocalResources";
            AddonsPath = FileSystemRoot + "/Addons";
            AddonsToolsPath = AddonsPath + "/Tools";
            AddonsPluginsPath = AddonsPath + "/Plugins";

            Directory.CreateDirectory(Root + FileSystemRoot);
            Directory.CreateDirectory(Root + BrushesPath);
            Directory.CreateDirectory(Root + LocalResources);
            Directory.CreateDirectory(Root + AddonsPath);
            Directory.CreateDirectory(Root + AddonsToolsPath);
            Directory.CreateDirectory(Root + AddonsPluginsPath);


            LoadLocalResources();
        }


        public void LoadLocalResources()
        {
            LoadImageFromResources("Brush1.png");
        }

        public void LoadImageFromResources(string imageName)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            BitmapImage bmi = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageName));

            encoder.Frames.Add(BitmapFrame.Create(bmi));
            using (var fileStream = new System.IO.FileStream(GetFullPath(LocalResources + "/" + imageName), System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
                Resources.Add(imageName, LocalResources + "/" + imageName);
            }
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(Root + path);
        }
    }
}

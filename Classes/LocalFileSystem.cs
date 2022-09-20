using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Paint.Classes
{
    public sealed class LocalFileSystem
    {
        public string Root { get; private set; }
        public string FileSystemRoot { get; private set; }
        public string BrushesPath { get; private set; }
        private string LocalResources { get; set; }

        public Dictionary<string, string> Resources { get; private set; } = new Dictionary<string, string>();

        public LocalFileSystem()
        {
            Root = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\");
            FileSystemRoot = "/Assets/";
            BrushesPath = FileSystemRoot + "/Brushes";
            LocalResources = FileSystemRoot + "/LocalResources";

            Directory.CreateDirectory(Root + FileSystemRoot);
            Directory.CreateDirectory(Root + BrushesPath);
            Directory.CreateDirectory(Root + LocalResources);


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
            return Root + path;
        }
    }
}

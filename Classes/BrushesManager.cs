using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Paint.Classes
{
    public sealed class BrushesManager
    {
        [System.Serializable]
        public class Brush
        {
            [JsonRequired]
            private string name;
            [JsonRequired]
            private string imageName;


            [JsonIgnore]
            private Bitmap brush;
            [JsonIgnore]
            public Bitmap BrushImage => brush;

            [JsonIgnore]
            public BitmapImage BrushBitmapImage { get; set; }
            [JsonIgnore]
            public string Path => imageName;
            [JsonIgnore]
            public string Name => name;




            public Brush() { }

            public Brush(string fileName, string brushName, LocalFileSystem localFileSystem)
            {
                name = brushName;
                imageName = fileName;
                LoadBrush(fileName, localFileSystem);
            }

            public bool LoadBrush(string fileName, LocalFileSystem localFileSystem)
            {
                var path = System.IO.Path.GetFullPath(localFileSystem.GetFullPath(fileName));
                if (File.Exists(path))
                {
                    brush = new Bitmap(path);
                    BrushBitmapImage = new BitmapImage(new Uri(path));

                    return true;
                }
                return false;
            }

            public bool LoadBrush(LocalFileSystem localFileSystem)
            {
                return LoadBrush(imageName, localFileSystem);
            }
        }

       

        private LocalFileSystem localFileSystem;

        private List<Brush> brushes = new List<Brush>();



        public BrushesManager(LocalFileSystem fileSystem)
        {
            localFileSystem = fileSystem;
            var brushesCount = BrushesLoader();
            if (brushesCount == 0)
            {
                var staticBrushPath = fileSystem.Resources["Brush1.png"];
                CreateBrush(localFileSystem.GetFullPath(staticBrushPath), "Standard");
            }
        }

        public List<Brush> GetBrushes() => new List<Brush>(brushes);

        public int BrushesLoader()
        {
            var brushesFolders = Directory.GetDirectories(localFileSystem.GetFullPath(localFileSystem.BrushesPath));
            foreach (var folder in brushesFolders)
            {
                var dataFile = folder + "/data.json";
                if (File.Exists(dataFile))
                {
                    var data = JsonConvert.DeserializeObject<Brush>(File.ReadAllText(dataFile));

                    if (data.LoadBrush(localFileSystem))
                    {
                        brushes.Add(data);
                    }
                }
            }
            return brushes.Count;
        }


        public void CreateBrush(string fileName, string brushName)
        {
            if (brushes.Find(x => x.Name == brushName) == null)
            {
                string path = $"{localFileSystem.GetFullPath(localFileSystem.BrushesPath)}/{brushName}";


                var filePath = path + $"/{brushName}.png";


                Directory.CreateDirectory(path);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Copy(fileName, filePath);


                var brush = new Brush($"{localFileSystem.BrushesPath}/{brushName}/{brushName}.png", brushName, localFileSystem);
                brushes.Add(brush);

                try
                {
                    var jsonBrush = JsonConvert.SerializeObject(brush);
                    File.WriteAllText(path + "/data.json", jsonBrush);
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }
    }
}

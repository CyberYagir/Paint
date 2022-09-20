using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
            public string Name => name;

            public Brush() { }

            public Brush(string fileName, string brushName)
            {
                name = brushName;
                imageName = fileName;
                LoadBrush(fileName);
            }

            public bool LoadBrush(string fileName)
            {
                if (File.Exists(fileName))
                {
                    brush = new Bitmap(fileName);
                    return true;
                }
                return false;
            }

            public bool LoadBrush()
            {
                return LoadBrush(imageName);
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
                CreateBrush(staticBrushPath, "Standard");
            }
        }


        public int BrushesLoader()
        {
            var brushesFolders = Directory.GetDirectories(localFileSystem.GetFullPath(localFileSystem.BrushesPath));

            foreach (var folder in brushesFolders)
            {
                var dataFile = folder + "data.json";

                if (File.Exists(dataFile))
                {
                    var data = JsonConvert.DeserializeObject<Brush>(File.ReadAllText(dataFile));
                    if (data.LoadBrush())
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
                var brush = new Brush(fileName, brushName);
                brushes.Add(brush);

                Directory.CreateDirectory(path);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Copy(localFileSystem.GetFullPath(fileName), filePath);
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

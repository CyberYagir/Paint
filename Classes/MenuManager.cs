using Microsoft.Win32;
using Paint.Classes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Paint
{
    public class MenuManager
    {
        private MainWindow mainWindow;
        private AddonsLoader addonsLoader;

        public MenuManager(MainWindow main, AddonsLoader addonsLoader)
        {
            this.mainWindow = main;
            this.addonsLoader = addonsLoader;
            DrawPluginsButtons();
        }


        public void DrawPluginsButtons()
        {

            var addonsItem = FindMenuItem(mainWindow.MainMenu.Items, "Addons");
            if (addonsItem != null)
            {
                var pluginsItem = FindMenuItem(addonsItem.Items, "Plugins");
                if (pluginsItem != null)
                {
                    pluginsItem.Items.Clear();
                    var items = addonsLoader.GetPlugins().AllItems;
                    if (items.Count != 0)
                    {
                        pluginsItem.Click -= mainWindow.OpenPlugins;
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        var menuItem = new MenuItem()
                        {
                            Header = items[i].Name,
                            Foreground = pluginsItem.Foreground
                        };
                        int localID = i;
                        menuItem.Click += (a, e) =>
                        {
                            mainWindow.UndoRendo.AddAction();
                            items[localID].Call();
                        };

                        pluginsItem.Items.Add(menuItem);
                    }
                }
            }


            MenuItem FindMenuItem(ItemCollection itemCollection, string name)
            {
                foreach (var item in itemCollection)
                {
                    var menuItem = item as MenuItem;
                    if (menuItem != null && menuItem.Header is string)
                    {
                        if ((string)menuItem.Header == name)
                        {
                            return menuItem;
                        }
                    }
                }
                return null;
            }
        }


        public void OpenFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = FilterGenerator.GenerateFilter("Formats", "png", "jpg", "jpeg", "bmp");
            openFile.FilterIndex = 1;
            if ((bool)openFile.ShowDialog())
            {
                var bitmap = new Bitmap(openFile.FileName);

                Bitmap clone = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);

                using (Graphics gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(bitmap, new Rectangle(0, 0, clone.Width, clone.Height));
                }

                bitmap.Dispose();
                WriteableBitmap writeableBitmap = new WriteableBitmap(ToBitmapImage(clone));
                mainWindow.SetMainImage(writeableBitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }



        public void SaveFile(System.Windows.Media.ImageSource source)
        {
            CreateThumbnail((BitmapSource)source);
        }

        void CreateThumbnail(BitmapSource source)
        {
            if (source == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FilterGenerator.GenerateFilter("Formats", "jpg", "jpeg", "png", "bmp", "gif", "tiff", "wmp");
            if ((bool)saveFileDialog.ShowDialog())
            {

                if (saveFileDialog.FileName != string.Empty)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        BitmapEncoder enc = null;
                        Debug.Log(saveFileDialog.FilterIndex);
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 1:
                                enc = new JpegBitmapEncoder();
                                break;
                            case 2:
                                enc = new JpegBitmapEncoder();
                                break;
                            case 3:
                                enc = new PngBitmapEncoder();
                                break;
                            case 4:
                                enc = new BmpBitmapEncoder();
                                break;
                            case 5:
                                enc = new GifBitmapEncoder();
                                break;
                            case 6:
                                enc = new TiffBitmapEncoder();
                                break;
                            case 7:
                                enc = new WmpBitmapEncoder();
                                break;
                        }

                        enc.Frames.Add(BitmapFrame.Create(source));
                        enc.Save(stream);
                    }
                }
            }
        }
    }
}

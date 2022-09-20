using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Paint
{
    public class MenuManager
    {
        private Grid menu;
        private MainWindow mainWindow;
        public MenuManager(Grid menu, MainWindow main)
        {
            this.menu = menu;
            this.mainWindow = main;
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
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 0:
                                enc = new JpegBitmapEncoder();
                                break;
                            case 1:
                                enc = new JpegBitmapEncoder();
                                break;
                            case 2:
                                enc = new PngBitmapEncoder();
                                break;
                            case 3:
                                enc = new BmpBitmapEncoder();
                                break;
                            case 4:
                                enc = new GifBitmapEncoder();
                                break;
                            case 5:
                                enc = new TiffBitmapEncoder();
                                break;
                            case 6:
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

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
        private Menu menu;
        private MainWindow mainWindow;
        public MenuManager(Menu menu, MainWindow main)
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

        void CreateThumbnail(BitmapSource image5)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FilterGenerator.GenerateFilter("Formats", "jpg", "jpeg", "png", "bmp", "gif", "tiff", "wmp");
            if ((bool)saveFileDialog.ShowDialog())
            {

                if (saveFileDialog.FileName != string.Empty)
                {
                    using (FileStream stream5 = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        BitmapEncoder encoder5 = null;
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 0:
                                encoder5 = new JpegBitmapEncoder();
                                break;
                            case 1:
                                encoder5 = new JpegBitmapEncoder();
                                break;
                            case 2:
                                encoder5 = new PngBitmapEncoder();
                                break;
                            case 3:
                                encoder5 = new BmpBitmapEncoder();
                                break;
                            case 4:
                                encoder5 = new GifBitmapEncoder();
                                break;
                            case 5:
                                encoder5 = new TiffBitmapEncoder();
                                break;
                            case 6:
                                encoder5 = new WmpBitmapEncoder();
                                break;
                        }

                        encoder5.Frames.Add(BitmapFrame.Create(image5));
                        encoder5.Save(stream5);


                    }
                }
            }
        }
    }
}

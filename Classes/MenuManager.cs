using Microsoft.Win32;
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
                mainWindow.SetMainImage(new BitmapImage(new System.Uri(openFile.FileName)));
            }
        }

    }
}

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Paint;
using Paint.Classes;

namespace BrightnessPlugin
{
    /// <summary>
    /// Логика взаимодействия для BrightnessWindow.xaml
    /// </summary>
    public partial class BrightnessWindow : Window
    {
        MainWindow mainWindow;
        WriteableBitmap startBitMap;

        float percent = 1;

        public BrightnessWindow(MainWindow window)
        {
            mainWindow = window;
            startBitMap = mainWindow.PaintManager.GetBitMap();

            InitializeComponent();

            Percent.Content = "Brightness: 1";
        }

        public void UpdatePercent()
        {
            if (Percent != null)
            {
                Percent.Content = "Brightness: " + PercentSlider.Value.ToString("F2") + "%";
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (startBitMap != null)
            {
                percent = (float)PercentSlider.Value;
                SetNewImage();
                UpdatePercent();
            }
        }

        public void SetNewImage()
        {
            var newBitmap = startBitMap.Clone();
            mainWindow.PaintManager.SetWritableImage(newBitmap);

            var bitmap = newBitmap;
            bitmap.Lock();
            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    var pos = new YVector(x, y);
                    if (mainWindow.PaintManager.CanDrawRect(pos))
                    {
                        Color color = mainWindow.PaintManager.GetWritableBitmapColor(pos);
                        var alpha = color.A;
                        color = color * percent;

                        color = Color.FromArgb(alpha, color.R, color.G, color.B);

                        mainWindow.PaintManager.DrawPixel(pos, color);
                    }
                }
            }
            bitmap.Unlock();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var answer = MessageBox.Show("Should brightness changes be saved?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (answer == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (answer == MessageBoxResult.No)
            {
                mainWindow.PaintManager.SetWritableImage(startBitMap);
            }
            else
            {
                mainWindow.AddAction();
            }
            
        }
    }
}

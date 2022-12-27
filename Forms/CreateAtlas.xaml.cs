using Paint.Classes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint.Forms
{
    /// <summary>
    /// Логика взаимодействия для CreateAtlas.xaml
    /// </summary>
    public partial class CreateAtlasWindow : Window
    {
        MainWindow mainWindow;

        YVector size = new YVector(1000, 1000);

        public CreateAtlasWindow(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;

            var image = mainWindow.PaintManager.GetBitMap();

            if (image != null)
            {
                size = new YVector(image.PixelWidth, image.PixelHeight);
                WidthTextBox.Text = image.PixelWidth.ToString();
                HeightTextBox.Text = image.PixelHeight.ToString();
                UpdateSize();
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static WriteableBitmap CreateBlank(YVector size)
        {
            WriteableBitmap bitmapImage =
                new WriteableBitmap(size.XInt, size.YInt, 72, 72, PixelFormats.Pbgra32, null);

            int[] pixels = new int[(int)size.Mult];
            Color color = Color.FromArgb(255, 255, 255, 255);

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ((color.A << 24) + (color.R << 16) + (color.G << 8) + color.B);
            }
            bitmapImage.WritePixels(new Int32Rect(0, 0, size.XInt, size.YInt), pixels, bitmapImage.BackBufferStride, 0);

            return bitmapImage;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (size.XInt == 0 || size.YInt == 0)
            {
                MessageBox.Show("The image must be at least one pixel!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            mainWindow.SetMainImage(CreateBlank(size));
            Close();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WidthTextBox == null || HeightTextBox == null) return;

            var textBox = sender as TextBox;

            if (textBox.Text != "")
            {
                UpdateSize();
            }
            else
            {
                textBox.Text = "1";
            }
        }

        public void UpdateSize()
        {
            size.X = int.Parse(WidthTextBox.Text.Replace("\n", "").Replace("\r", ""));
            size.Y = int.Parse(HeightTextBox.Text.Replace("\n", "").Replace("\r", ""));
        }
    }
}

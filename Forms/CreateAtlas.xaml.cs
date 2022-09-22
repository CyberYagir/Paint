using Paint.Classes;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
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

        YVector size = new YVector();

        public CreateAtlasWindow(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
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

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (size.XInt == 0 && size.YInt == 0) return;

            WriteableBitmap bitmapImage =
                new WriteableBitmap(size.XInt, size.YInt, 72, 72, PixelFormats.Pbgra32, null);

            int[] pixels = new int[(int)size.Mult];
            Color color = Color.FromArgb(255, 255, 255, 255);

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ((color.A << 24) + (color.R << 16) + (color.G << 8) + color.B);
            }
            bitmapImage.WritePixels(new Int32Rect(0, 0, size.XInt, size.YInt), pixels, bitmapImage.BackBufferStride, 0);


            mainWindow.SetMainImage(bitmapImage);

            Close();
        }

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (textBox.Text != "")
            {
                size.X = int.Parse(textBox.Text.Replace("\n", "").Replace("\r", ""));
            }
            else
            {
                size.X = 0;
            }
        }
        private void textBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (textBox1.Text != "")
            {
                size.Y = int.Parse(textBox1.Text.Replace("\n", "").Replace("\r", ""));
            }
            else
            {
                size.Y = 0;
            }
        }
    }
}

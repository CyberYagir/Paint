using Microsoft.Win32;
using Paint.Classes;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Paint.Forms
{
    /// <summary>
    /// Логика взаимодействия для Brushes.xaml
    /// </summary>
    public partial class BrushesWindow : Window
    {
        private string newBrushPath;
        private BrushesManager brushes;
        private LocalFileSystem fileSystem;
        private MainWindow mainWindow;
        public BrushesWindow(BrushesManager brushesManager, LocalFileSystem fileSystem, MainWindow mainWindow)
        {
            InitializeComponent();
            brushes = brushesManager;
            this.fileSystem = fileSystem;
            this.mainWindow = mainWindow;
            var brushesList = brushes.GetBrushes();
            DrawBrushes(brushesList, fileSystem);

            int offcet = 0;
            string nameText = "";

            do
            {
                nameText = "New_Brush_" + (brushesList.Count + offcet);
                offcet++;
            } while (brushesList.Find(x=>x.Name == nameText) != null);

            NameBrush.Text = nameText;
        }

        public void DrawBrushes(List<BrushesManager.Brush> brushes, LocalFileSystem fileSystem)
        {
            foreach (Grid item in List.Children)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach (var item in brushes)
            {
                var clone = CloneElement(Item) as Grid;


                (clone.Children[0] as Image).Source = new BitmapImage(new System.Uri(fileSystem.GetFullPath(item.Path)));
                (clone.Children[1] as Label).Content = item.Name;

                clone.Visibility = Visibility.Visible;

                clone.MouseEnter += Item_MouseEnter;
                clone.MouseLeave += Item_MouseLeave;
                clone.MouseDown += Item_MouseDown;
                clone.DataContext = item;
                List.Children.Add(clone);
            }
        }


        public UIElement CloneElement(Grid orig)
        {
            if (orig == null)
                return (null);
            string s = XamlWriter.Save(orig);
            StringReader stringReader = new StringReader(s);
            XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());
            return (UIElement)XamlReader.Load(xmlReader);
        }

        private void Item_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Grid).Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        }
        private void Item_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Grid).Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        }

        private void CreateBrush_Click(object sender, RoutedEventArgs e)
        {
            if (newBrushPath != null)
            {
                if (NameBrush.Text.Trim() != "")
                {
                    if (File.Exists(newBrushPath))
                    {
                        var bitmap = new BitmapImage(new System.Uri(newBrushPath));

                        if ((int)bitmap.Width != (int)bitmap.Height)
                        {
                            MessageBox.Show("Upload an image with a 1:1 ratio!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        if ((int)bitmap.Width > 512)
                        {
                            MessageBox.Show("Image is too big! 512 Max!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            if (brushes.GetBrushes().Find(x => x.Name.Trim() == NameBrush.Text.Trim()) == null)
                            {
                                brushes.CreateBrush(newBrushPath, NameBrush.Text.Trim());
                                DrawBrushes(brushes.GetBrushes(), fileSystem);
                            }
                            else
                            {
                                MessageBox.Show("Brush Exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        }

        private void OpenBrush_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = fileSystem.GetFullPath(fileSystem.BrushesPath);
            openFileDialog.Filter = FilterGenerator.GenerateFilter("File Type", "png");
            if ((bool)openFileDialog.ShowDialog())
            {
                if (Path.GetExtension(openFileDialog.FileName) == ".png")
                {
                    newBrushPath = openFileDialog.FileName;
                    OpenBrush.Content = Path.GetFileName(newBrushPath);
                }
                else
                {
                    MessageBox.Show("Select a png file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    newBrushPath = null;
                }
            }
        }

        private void Item_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindow.CurrentBrush = ((sender as Grid).DataContext as BrushesManager.Brush);
            Close();
        }
    }
}

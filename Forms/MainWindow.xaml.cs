using Paint.Classes;
using Paint.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Paint
{
    public partial class MainWindow : Window
    {
        private MenuManager menuManager;
        private PaintManager paintManager;
        private LocalFileSystem localSystem;
        private BrushesManager brushesManager;

        Vector windowSize;

        public MainWindow()
        {
            InitializeComponent();
            localSystem = new LocalFileSystem();

            menuManager = new MenuManager(menu, this);
            paintManager = new PaintManager(MainImage, frame, this);
            brushesManager = new BrushesManager(localSystem);
            windowSize = new Vector(this.ActualHeight, this.Height);
        }

        #region Main
        internal void SetMainImage(WriteableBitmap bitmapImage)
        {
            MainImage.Source = bitmapImage;
            MainImage.Width = bitmapImage.Width;
            MainImage.Height = bitmapImage.Height;
            paintManager.UploadImage(bitmapImage);
        }

        #endregion

        #region Menu
        public void OpenFile(object sender, RoutedEventArgs e) => menuManager.OpenFile();

        private void FileContext(object sender, RoutedEventArgs e)
        {
            FileMenu.Visibility = Visibility.Visible;
            double y = Mouse.GetPosition(this).Y;
            if (y < menu.Height)
            {
                y = menu.Height;
            }
            FileMenu.Margin = new Thickness(Mouse.GetPosition(this).X, y, 0, 0);
        }
        private void FileMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Visibility = Visibility.Hidden;
        }
        private void Createbtn_Click(object sender, RoutedEventArgs e)
        {
            CreateAtlasWindow createWindow = new CreateAtlasWindow(this);
            createWindow.ShowDialog();
        }
        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            menuManager.SaveFile(MainImage.Source);
        }


        #endregion

        #region Window
        private void frame_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var framePos = new YVector(e.GetPosition(frame));
            paintManager.SetStartPos(framePos);
            paintManager.SetState(PaintManager.State.Moving);
        }
        private void frame_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintManager.SetState(PaintManager.State.Paint);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var framePos = new YVector(e.GetPosition(frame));
            var imagePos = new YVector(e.GetPosition(MainImage));
            paintManager.Update(imagePos, framePos, ColorPicker.SelectedColor, e);
        }
        private void frame_MouseLeave(object sender, MouseEventArgs e)
        {

            paintManager.SetState(PaintManager.State.Paint);
        }
        private void frame_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            paintManager.Scale(e.Delta);

        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0) return;
            var delta = new YVector(e.PreviousSize.Width - e.NewSize.Width, e.PreviousSize.Height - e.NewSize.Height);
            paintManager.ReducePos(delta / 2f);
        }

        #endregion

        #region SideMenu

        #region BrushSize
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSizeLabel.Content = "Brush Size: " + (int)BrushSizeSlider.Value;
        }

        #endregion

        #region BrushSelection
        private void SelectBrushButton_Click(object sender, RoutedEventArgs e)
        {
            BrushesWindow brushesWindow = new BrushesWindow(brushesManager);
            brushesWindow.ShowDialog();
        }

        #endregion

        #endregion

    }
}

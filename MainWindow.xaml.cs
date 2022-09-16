using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint
{
    public partial class MainWindow : Window
    {
        private MenuManager menuManager;
        private PaintManager paintManager;


        public MainWindow()
        {
            InitializeComponent();
            menuManager = new MenuManager(menu, this);
            paintManager = new PaintManager(MainImage, frame, this);
        }

        internal void SetMainImage(WriteableBitmap bitmapImage)
        {
            MainImage.Source = bitmapImage;
            MainImage.Width = bitmapImage.Width;
            MainImage.Height = bitmapImage.Height;
            paintManager.UploadImage(bitmapImage);
        }


        public void OpenFile(object sender, RoutedEventArgs e) => menuManager.OpenFile();

        private void frame_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var framePos = (Vector)e.GetPosition(frame);
            paintManager.SetStartPos(framePos);
            paintManager.SetState(PaintManager.State.Moving);
        }

        private void frame_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintManager.SetState(PaintManager.State.Paint);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var framePos = (Vector)e.GetPosition(frame);
            var imagePos = (Vector)e.GetPosition(MainImage);
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

        private void Window_LayoutUpdated(object sender, System.EventArgs e)
        {

        }

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
    }
}

using Paint.Classes;
using Paint.Forms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Paint
{
    public partial class MainWindow : Window
    {
        private MenuManager menuManager;
        private PaintManager paintManager;
        private LocalFileSystem localSystem;
        private BrushesManager brushesManager;
        private UndoRendo undoRendo;
        private InstrumentsLoader instrumentsLoader;
        private HotKeysManager keysManager;


        BrushesManager.Brush brush;
        DispatcherTimer timer;


        public PaintManager PaintManager => paintManager;
        public InstrumentsLoader InstrumentsLoader => instrumentsLoader;


        public BrushesManager.Brush CurrentBrush
        {
            get
            {
                return brush;
            }
            set
            {
                brush = value;
                BrushPreview.Source = value.BrushBitmapImage;
                BrushImage.Source = value.BrushBitmapImage;
            }
        }
        


        public MainWindow()
        {
            InitializeComponent();
            localSystem = new LocalFileSystem();

            menuManager = new MenuManager(this);
            paintManager = new PaintManager(MainImage, frame, this);

            instrumentsLoader = new InstrumentsLoader(localSystem, this);
            
            brushesManager = new BrushesManager(localSystem);
            undoRendo = new UndoRendo(paintManager);

            keysManager = new HotKeysManager();
            AddAllHotKeys();

            paintManager.SetInstrumentsList(instrumentsLoader.GetInstruments());
            CurrentBrush = brushesManager.GetBrushes()[0];


            StartWindowConfiguration();
        }

        public void AddAllHotKeys()
        {
            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Z, delegate
            {
                undoRendo.Undo();
            }));
            keysManager.AddHotKey(new HotKey(ModifierKeys.Control | ModifierKeys.Shift, Key.Z, delegate
            {
                undoRendo.Redo();
            }));
            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Y, delegate
            {
                undoRendo.Redo();
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Y, delegate
            {
                undoRendo.Redo();
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.P, delegate
            {
                OpenPlugins(PluginsWindow.DisplayState.All);
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.B, delegate
            {
                SelectBrushButton_Click(null, new RoutedEventArgs());
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.O, delegate
            {
                Createbtn_Click(null, new RoutedEventArgs());
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.N, delegate
            {
                OpenFile(null, new RoutedEventArgs());
            }));

            keysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.S, delegate
            {
                Savebtn_Click(null, new RoutedEventArgs());
            }));
        }


        #region Main

        public void StartWindowConfiguration()
        {

            FillImage.Visibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += (a, e) =>
            {
                SetMainImage(CreateAtlasWindow.CreateBlank(new YVector(1000, 1000)));
                timer.Stop();
            };
            timer.Start();

            ColorPicker.SelectedColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);

        }

        public void SetMainImage(WriteableBitmap bitmapImage)
        {
            MainImage.Source = bitmapImage;
            MainImage.Width = bitmapImage.Width;
            MainImage.Height = bitmapImage.Height;
            paintManager.UploadImage(bitmapImage);
            undoRendo.Clear();
            undoRendo.AddAction();
        }

        public void SetBrushImage(YVector pos)
        {
            BrushImage.Width = brush.BrushBitmapImageScaled.Width * paintManager.Zoom;
            BrushImage.Height = brush.BrushBitmapImageScaled.Height * paintManager.Zoom;
            BrushImage.Opacity = ColorPicker.Color.A / 255;

            if (pos != null)
            {
                BrushImage.Margin = new Thickness(pos.X - BrushImage.Width / 2f, pos.Y - BrushImage.Height / 2f, 0, 0);
            }
        }

        #endregion

        #region Menu
        public void OpenFile(object sender, RoutedEventArgs e) => menuManager.OpenFile();
        private void OpenPlugins(PluginsWindow.DisplayState state)
        {
            PluginsWindow window = new PluginsWindow(this, state);
            window.ShowDialog();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void OpenToolsBtn(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.Instruments);
        }

        private void OpenPlugins(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.Plugins);
        }

        private void OpenAddons(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.All);
        }
        private void CreatorButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Creator: Yaroslav Khromov", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void GitHub(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/CyberYagir");
        }
        #endregion

        #region Window


        private void frame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (paintManager.IsChanged)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                timer.Tick += (a, g) =>
                {
                    AddAction();
                    timer.Stop();
                };

                timer.Start();
            }
        }
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
        public void AddAction()
        {
            undoRendo.AddAction();
            UpdateUndoRendoDebug();
        }
        public void UpdateUndoRendoDebug()
        {
            return;
            var list = undoRendo.Bitmaps;
            for (int i = 0; i < UndoRendoDebug.Children.Count; i++)
            {
                if (i < list.Count)
                {
                    (UndoRendoDebug.Children[i] as Image).Source = list[i];
                }
                else
                {
                    (UndoRendoDebug.Children[i] as Image).Source = null;
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var framePos = new YVector(e.GetPosition(frame));
            var imagePos = new YVector(e.GetPosition(MainImage));
            paintManager.Update(imagePos, framePos, ColorPicker.SelectedColor, e);
            SetBrushImage(new YVector(e.GetPosition(frame)));
            if (paintManager.CurrentInstrument == "Fill")
            {
                FillImage.Margin = BrushImage.Margin;
            }
        }
        private void frame_MouseLeave(object sender, MouseEventArgs e)
        {
            paintManager.SetState(PaintManager.State.Paint);
        }
        private void frame_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            paintManager.Scale(e.Delta);
            SetBrushImage(new YVector(Mouse.GetPosition(frame)));

        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0) return;
            var delta = new YVector(e.PreviousSize.Width - e.NewSize.Width, e.PreviousSize.Height - e.NewSize.Height);
            paintManager.ReducePos(delta / 2f);
        }

        private void frame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var framePos = new YVector(e.GetPosition(frame));
            var imagePos = new YVector(e.GetPosition(MainImage));
            paintManager.Update(imagePos, framePos, ColorPicker.SelectedColor, e);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            undoRendo.Undo();
            UpdateUndoRendoDebug();
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            undoRendo.Redo();
            UpdateUndoRendoDebug();
        }

        #endregion

        #region SideMenu

        #region Brush

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            if (paintManager.CurrentInstrument != "Fill")
            {
                paintManager.SetInstrument("Fill");
            }
            else
            {
                paintManager.SetInstrument("Brush");
            }

            var isFillOn = paintManager.CurrentInstrument == "Fill";

            FillIcon.Visibility = isFillOn ? Visibility.Visible : Visibility.Hidden;
            BrushImage.Visibility = isFillOn ? Visibility.Hidden : Visibility.Visible;

            FillImage.Visibility = !isFillOn ? Visibility.Hidden : Visibility.Visible;

        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSizeLabel.Content = "Brush Size: " + (BrushSizeSlider.Value * 100f).ToString("F2") + "%";
            
        }
        private void BrushSizeSlider_MouseUp(object sender, DragCompletedEventArgs e)
        {
            if (CurrentBrush != null)
            {
                brushesManager.ChangeBrushScale((float)BrushSizeSlider.Value);
                BrushImage.Width = CurrentBrush.BrushBitmapImageScaled.Width;
                BrushImage.Height = CurrentBrush.BrushBitmapImageScaled.Height;
            }
        }

        #endregion

        #region BrushSelection
        private void SelectBrushButton_Click(object sender, RoutedEventArgs e)
        {
            BrushesWindow brushesWindow = new BrushesWindow(brushesManager, localSystem, this);
            brushesWindow.ShowDialog();
        }





        #endregion

        private void FillDelta_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (paintManager != null)
            {
                paintManager.SetFillPrecision((int)FillDelta.Value / 254f);
                FillDeltaText.Content = (int)FillDelta.Value;
            }
        }





        #endregion

        private void HotKeys(object sender, KeyEventArgs e)
        {
            keysManager.CheckHotKeys(e);
        }


    }
}

using Paint.Classes;
using Paint.Forms;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Paint
{
    public partial class MainWindow : Window
    {
        public MenuManager MenuManager { get; private set; }
        public PaintManager PaintManager { get; private set; }
        public LocalFileSystem LocalSystem { get; private set; }
        public BrushesManager BrushesManager { get; private set; }
        public UndoRendo UndoRendo { get; private set; }
        public ToolsLoader InstrumentsLoader { get; private set; }
        public HotKeysManager KeysManager { get; private set; }


        BrushesManager.Brush brush;
        DispatcherTimer timer;

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
            LocalSystem = new LocalFileSystem();

            MenuManager = new MenuManager(this);
            PaintManager = new PaintManager(MainImage, frame, this);

            InstrumentsLoader = new ToolsLoader(LocalSystem, this);

            BrushesManager = new BrushesManager(LocalSystem);
            UndoRendo = new UndoRendo(PaintManager);

            KeysManager = new HotKeysManager();
            AddAllHotKeys();

            PaintManager.SetInstrumentsList(InstrumentsLoader.GetInstruments());
            CurrentBrush = BrushesManager.GetBrushes().Find(x=>x.Name == "Standard");


            StartWindowConfiguration();
        }



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


            BrushImage.Effect = new DropShadowEffect()
            {
                Opacity = 0.2f
            };

            FillImage.Effect = new DropShadowEffect()
            {
                Opacity = 0.2f
            };
        }

        public void SetMainImage(WriteableBitmap bitmapImage)
        {
            MainImage.Source = bitmapImage;
            MainImage.Width = bitmapImage.Width;
            MainImage.Height = bitmapImage.Height;
            PaintManager.UploadImage(bitmapImage);
            UndoRendo.Clear();
            UndoRendo.AddAction();
        }

        public void SetBrushImage(YVector pos)
        {
            BrushImage.Width = brush.BrushBitmapImageScaled.Width * PaintManager.Zoom;
            BrushImage.Height = brush.BrushBitmapImageScaled.Height * PaintManager.Zoom;
            BrushImage.Opacity = ColorPicker.Color.A / 255;
            BrushImage.Effect = new DropShadowEffect()
            {
                Opacity = (0.2f/ BrushImage.Opacity)
            };

            if (pos != null)
            {
                BrushImage.Margin = new Thickness(pos.X - BrushImage.Width / 2f, pos.Y - BrushImage.Height / 2f, 0, 0);
            }
        }


        private void HotKeys(object sender, KeyEventArgs e)
        {
            KeysManager.CheckHotKeys(e);
        }


    }
}

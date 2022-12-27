using System.Windows.Media;

namespace Paint.Classes.Instruments
{
    class PluginColorInvertor : Plugin
    {

        public override void Init(MainWindow mainWindow)
        {
            base.Init(mainWindow);
            name = "Invert Colors";
        }

        public override void Call()
        {
            var bitmap = window.PaintManager.GetOrginalBitmap();
            bitmap.Lock();
            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    var pos = new YVector(x, y);
                    if (window.PaintManager.CanDrawRect(pos))
                    {
                        Color inv = window.PaintManager.GetWritableBitmapColor(pos);
                        inv = Color.FromArgb(inv.A, (byte)(255 - inv.R), (byte)(255 - inv.G), (byte)(255 - inv.B));
                        window.PaintManager.DrawPixel(pos, inv);
                    }
                }
            }
            bitmap.Unlock();
        }
    }
}

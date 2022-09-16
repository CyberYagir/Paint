using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO;

namespace Paint
{
    public class PaintManager
    {
        public enum State
        {
            Paint, Moving
        }

        private State state;

        private PixelColor[,] colors;
        WriteableBitmap bitmapImage = null;
        private Vector startPos;
        private Thickness imagePos = new Thickness();

        private Image mainImage;
        private Frame frame;
        private float scale = 1f;
        private MainWindow window;

        Vector lastLocalImageMousePos;
        Vector lastLocalRectMousePos;

        public float Zoom => scale;

        public PaintManager(Image mainImage, Frame frame, MainWindow window)
        {
            this.mainImage = mainImage;
            this.window = window;
            this.frame = frame;
            imagePos = mainImage.Margin;
        }

        public void SetState(State state)
        {
            if (this.state == State.Moving && state == State.Paint)
            {
                imagePos = mainImage.Margin;
            }
            this.state = state;
        }
        public void SetStartPos(Vector startFramePos) => this.startPos = startFramePos;


        public void Update(Vector imageMousePos, Vector frameMousePos)
        {
            lastLocalImageMousePos = imageMousePos;
            lastLocalRectMousePos = frameMousePos;
            switch (state)
            {
                case State.Paint:
                    Draw(lastLocalImageMousePos);
                    break;
                case State.Moving:
                    Move(frameMousePos);
                    break;
                default:
                    break;
            }
        }


        public void Draw(Vector imagePixel)
        {
            if (bitmapImage != null)
            {
                imagePixel /= scale;

                if (imagePixel.X >= 0 && imagePixel.X < bitmapImage.PixelWidth)
                {
                    if (imagePixel.Y >= 0 && imagePixel.Y < bitmapImage.PixelHeight)
                    {

                        colors[(int)imagePixel.X, (int)imagePixel.Y] = new PixelColor(0, 0, 0, 255);

                        PutPixels(bitmapImage, colors, 0, 0);
                        UploadImage(ConvertWriteableBitmapToBitmapImage(bitmapImage), true);
                    }
                }
            }
        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        public void Move(Vector frameMousePos)
        {
            mainImage.Margin = imagePos;
            var delta = startPos - frameMousePos;

            var newPos = new Thickness(imagePos.Left - delta.X, imagePos.Top - delta.Y, 0, 0);

            newPos.Left = Clamp(newPos.Left, -(mainImage.Width - 10), (window.ActualWidth) - 150);
            newPos.Top = Clamp(newPos.Top, -(mainImage.Height - 10), (window.ActualHeight) - 100);

            mainImage.Margin = newPos;
        }

        public void UploadImage(WriteableBitmap bitmapImage, bool notReset = false)
        {

            colors = new ArrayConverter<PixelColor>().TwoDimesional(GetPixels(bitmapImage), bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            if (!notReset)
            {
                this.bitmapImage = bitmapImage;
                scale = 1f;
                while (mainImage.Width > window.ActualWidth)
                {
                    scale -= 0.01f;
                    zoomStep -= 0.001f;
                    mainImage.Width = colors.GetLength(0) * scale;
                    mainImage.Height = colors.GetLength(1) * scale;
                }
                imagePos.Left = 0;
                imagePos.Top -= mainImage.Height * 0.25f;

                mainImage.Margin = imagePos;
            }
        }

        public double Clamp(double val, double min, double max)
        {
            if (val > max)
            {
                val = max;
            }
            if (val < min)
            {
                val = min;
            }
            return val;
        }

        float zoomStep = 0.2f;

        public void Scale(int delta)
        {
            if (state == State.Paint)
            {
                if (mainImage.Source == null) return;

                Vector oldScale = new Vector(mainImage.Width, mainImage.Height);

                scale += (delta / 120f) * zoomStep;
                if (scale <= zoomStep)
                {
                    scale = zoomStep;
                }
                else
                {
                    var deltaVector = new Vector(colors.GetLength(0) * scale, colors.GetLength(1) * scale) - oldScale;


                    imagePos = new Thickness(imagePos.Left - (deltaVector.X / 2f), imagePos.Top - (deltaVector.Y / 2f), 0, 0);
                    mainImage.Margin = imagePos;
                    mainImage.Width = colors.GetLength(0) * scale;
                    mainImage.Height = colors.GetLength(1) * scale;



                    //mainImage.BeginAnimation(Image.MarginProperty, thicknessAnimation);
                    //Animation(colors.GetLength(0) * scale, (float)mainImage.Width, Image.WidthProperty);
                    //Animation(colors.GetLength(1) * scale, (float)mainImage.Height, Image.HeightProperty);
                }
            }

            //ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
            //thicknessAnimation.From = mainImage.Margin;
            //thicknessAnimation.To = imagePos;
            //thicknessAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(350));


            
        }


        public void Animation(float to, float from, DependencyProperty property)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = from;
            anim.To = to;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(350));
            mainImage.BeginAnimation(property, anim);

        }

        public struct PixelColor
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Alpha;

            public PixelColor(byte b, byte g, byte r, byte a)
            {
                Red = r;
                Green = g;
                Blue = b;
                Alpha = a;
            }

            internal string Log()
            {
                return $"RGB({Red},{Green},{Blue},{Alpha})";
            }
        }


        public List<PixelColor> GetPixels(BitmapSource source)
        {
            var bytes = GetBytesFromBitmapSource(source);
            List<PixelColor> pixelColors = new List<PixelColor>(source.PixelHeight * source.PixelWidth);

            for (int i = 0; i < bytes.Length; i += 4)
            {
                pixelColors.Add(new PixelColor(bytes[i + 0], bytes[i + 1], bytes[i + 2], bytes[i + 3]));
            }
            return pixelColors;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static byte[] GetBytesFromBitmapSource(BitmapSource bmp)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            var pixels = new byte[height * stride];

            bmp.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        public void PutPixels(WriteableBitmap bitmap, PixelColor[,] pixels, int x, int y)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, x, y);
        }
    }
}

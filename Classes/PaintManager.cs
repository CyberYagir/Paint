using Paint.Classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Paint
{
    public class PaintManager
    {
        public enum State
        {
            Paint, Moving
        }

        private State state;
        YVector initSize;


        WriteableBitmap bitmapImage = null;
        private YVector startPos;
        private Thickness imagePos = new Thickness();

        private Image mainImage;
        private Grid frame;
        private float scale = 1f;
        private MainWindow window;

        YVector lastLocalImageMousePos;
        YVector lastLocalRectMousePos;

        public float Zoom => scale;

        public PaintManager(Image mainImage, Grid frame, MainWindow window)
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
        public void SetStartPos(YVector startFramePos) => this.startPos = startFramePos;


        public void Update(YVector imageMousePos, YVector frameMousePos, Color color, MouseEventArgs e)
        {
            lastLocalImageMousePos = imageMousePos;
            lastLocalRectMousePos = frameMousePos;
            switch (state)
            {
                case State.Paint:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Draw(lastLocalImageMousePos, color);
                    }
                    break;
                case State.Moving:
                    Move(frameMousePos);
                    break;
                default:
                    break;
            }
        }

        // A cache of all opacity values (0-255) scaled down to 0-1 for performance
        private readonly float[] _opacities = Enumerable.Range(0, 256)
                                              .Select(o => o / 255f)
                                              .ToArray();

        private Color AlphaComposite(Color c1, Color c2)
        {
            var opa1 = _opacities[c1.A];
            var opa2 = _opacities[c2.A];
            var ar = opa1 + opa2 - (opa1 * opa2);
            var asr = opa2 / ar;
            var a1 = 1 - asr;
            var a2 = asr * (1 - opa1);
            var ab = asr * opa1;
            var r = (byte)(c1.R * a1 + c2.R * a2 + c2.R * ab);
            var g = (byte)(c1.G * a1 + c2.G * a2 + c2.G * ab);
            var b = (byte)(c1.B * a1 + c2.B * a2 + c2.B * ab);
            return Color.FromArgb((byte)(ar * 255), r, g, b);
        }
        public void Draw(YVector imagePixel, Color color)
        {
            if (bitmapImage != null)
            {
                imagePixel /= scale;
                YVector pos = new YVector(imagePixel.X, imagePixel.Y);

                bitmapImage.Lock();
                var bitmap = window.CurrentBrush.BrushBitmapImage;
                for (int x = -(int)bitmap.PixelWidth / 2; x < (bitmap.Width / 2) -1; x++)
                {
                    for (int y = -(int)bitmap.Height / 2; y < (bitmap.Height / 2) -1; y++)
                    {
                        var forPos = new YVector(x, y);
                        pos = imagePixel + forPos;
                        if (pos.X >= 0 && pos.X < bitmapImage.PixelWidth)
                        {
                            if (pos.Y >= 0 && pos.Y < bitmapImage.PixelHeight)
                            {
                                var posOnBrush = forPos + new YVector(bitmap.PixelWidth / 2, bitmap.PixelHeight/2);

                                var pixColorBrush = window.CurrentBrush.BrushImage.GetPixel(posOnBrush.XInt, posOnBrush.YInt);
                                //colorBrush
                                var c0 = Color.FromArgb((byte)(pixColorBrush.A * (color.A/255f)), color.R, color.G, color.B);

                                try
                                {
                                    // Reserve the back buffer for updates.

                                    unsafe
                                    {
                                        IntPtr pBackBuffer = bitmapImage.BackBuffer;

                                        byte* pBuff = (byte*)pBackBuffer.ToPointer();

                                        var b = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride)];
                                        var g = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 1];
                                        var r = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 2];
                                        var a = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 3];

                                        //ImageColor
                                        var imageColor = Color.FromArgb(a, r, g, b);

                                        //ImageBrush
                                        var c1 = Color.FromArgb(imageColor.A, imageColor.R, imageColor.G, imageColor.B);

                                        var final = AlphaComposite(c1, c0);

                                        DrawPixel(pos, final);
                                    }
                                }
                                catch
                                {
                                    //ignore
                                }
                            }
                        }
                    }
                }
                bitmapImage.Unlock();
            }
        }

        public void DrawPixel(YVector pos, Color color)
        {
            int column = pos.XInt;
            int row = pos.YInt;

            try
            {
                // Reserve the back buffer for updates.

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = bitmapImage.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += row * bitmapImage.BackBufferStride;
                    pBackBuffer += column * 4;

                    // Compute the pixel's color.
                    int color_data = (color.A << 24) + (color.R << 16) + (color.G << 8) + color.B;

                    // Assign the color data to the pixel.
                    *(int*)pBackBuffer = color_data;
                }
                // Specify the area of the bitmap that changed.
                bitmapImage.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
            }
        }

        internal void ReducePos(YVector pixels)
        {
            mainImage.BeginAnimation(FrameworkElement.MarginProperty, null);
            imagePos.Left -= pixels.X;
            imagePos.Top -= pixels.Y;
            mainImage.Margin = imagePos;
        }

        public void Move(YVector frameMousePos)
        {
            if (notAnimated)
            {
                mainImage.Margin = imagePos;
                var delta = startPos - frameMousePos;

                var newPos = new Thickness(imagePos.Left - delta.X, imagePos.Top - delta.Y, 0, 0);

                newPos.Left = Clamp(newPos.Left, -(mainImage.Width - 10), (window.ActualWidth) - 150);
                newPos.Top = Clamp(newPos.Top, -(mainImage.Height - 10), (window.ActualHeight) - 100);
                mainImage.BeginAnimation(FrameworkElement.MarginProperty, null);
                mainImage.Margin = newPos;
            }
        }

        public void UploadImage(WriteableBitmap bitmapImage)
        {
            mainImage.BeginAnimation(Image.MarginProperty, null);
            mainImage.BeginAnimation(Image.WidthProperty, null);
            mainImage.BeginAnimation(Image.HeightProperty, null);

            this.bitmapImage = bitmapImage;

            scale = 1f;
            zoomStep = 0.2f;

            initSize = new YVector(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            mainImage.InvalidateVisual();

            bool isBigger = mainImage.Width > window.ActualWidth;

            if (isBigger)
            {
                while (mainImage.Width > window.ActualWidth)
                {
                    scale -= 0.01f;
                    zoomStep -= 0.001f;
                    mainImage.Width = initSize.X * scale;
                    mainImage.Height = initSize.Y * scale;
                }
            }
            else
            {
                if (mainImage.Width > 10)
                {
                    while (mainImage.Width < window.ActualWidth + 100)
                    {
                        scale += 0.01f;
                        zoomStep += 0.001f;
                        mainImage.Width = initSize.X * scale;
                        mainImage.Height = initSize.Y * scale;
                    }
                }
            }



            imagePos.Left = (frame.ActualWidth - (initSize.X * scale)) / 2f;
            imagePos.Top = (frame.ActualHeight - (initSize.Y * scale)) / 2f;

            mainImage.Margin = imagePos;
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
            if (state == State.Paint && notAnimated)
            {
                if (mainImage.Source == null) return;

                Vector oldScale = new Vector(mainImage.Width, mainImage.Height);

                scale += (delta / 120f) * zoomStep;
                if (scale <= zoomStep)
                {
                    scale = zoomStep;
                }

                {
                    var deltaVector = new Vector(initSize.X * scale, initSize.Y * scale) - oldScale;
                    var newPos = new Thickness(imagePos.Left - (deltaVector.X / 2f), imagePos.Top - (deltaVector.Y / 2f), 0, 0);
                    ThicknessAnimation thicknessAnimation = new ThicknessAnimation()
                    {
                        Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
                        From = imagePos,
                        To = newPos
                    };
                    imagePos = newPos;

                    notAnimated = false;
                    mainImage.BeginAnimation(Image.MarginProperty, thicknessAnimation);
                    Animation(Image.WidthProperty, mainImage.Width, initSize.X * scale);
                    Animation(Image.HeightProperty, mainImage.Height, initSize.Y * scale);

                    //mainImage.Width = initSize.X * scale;
                    //mainImage.Height = initSize.Y * scale;
                }
            }
        }

        public float animationTime { get; } = 250;
        bool notAnimated = true;
        public void Animation(DependencyProperty property, double from, double to)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = from;
            anim.To = to;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(animationTime));
            anim.Completed += (a,g) => { notAnimated = true; };
            mainImage.BeginAnimation(property, anim);

        }
    }
}

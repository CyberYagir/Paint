using Paint.Classes;
using Paint.Classes.Instruments;
using System;
using System.Collections.Generic;
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


        List<Tool> instrumentsList = new List<Tool>();

        string currentInstrument;


        private State state;

        YVector initSize;

        Color currentColor;

        WriteableBitmap bitmapImage = null;
        private YVector startPos;
        private Thickness imagePos = new Thickness();

        private Image mainImage;
        private Grid frame;

        bool changed;

        private float scale = 1f;
        private float fillPrecision = 1;
        private MainWindow window;

        YVector lastLocalImageMousePos;
        YVector lastLocalRectMousePos;


        public float Zoom => scale;
        public bool IsChanged => changed;
        public State CurrentState => state;
        public Color CurrentColor => currentColor;

        public string CurrentInstrument => currentInstrument;

        public PaintManager(Image mainImage, Grid frame, MainWindow window)
        {
            this.mainImage = mainImage;
            this.window = window;
            this.frame = frame;
            imagePos = mainImage.Margin;
        }

        public void SetInstrumentsList(List<Tool> instruments)
        {
            instrumentsList = instruments;
            SetInstrument("Brush");
        }

        public void SetState(State state)
        {
            if (this.state == State.Moving && state == State.Paint)
            {
                imagePos = mainImage.Margin;
            }
            if (state == State.Paint && changed)
            {
                changed = false;
            }
            this.state = state;
        }
        public void SetStartPos(YVector startFramePos) => this.startPos = startFramePos;

        public void SetFillPrecision(float val)
        {
            fillPrecision = val;
        }

        public void Update(YVector imageMousePos, YVector frameMousePos, Color color, MouseEventArgs e, bool overSideBar)
        {
            lastLocalImageMousePos = imageMousePos;
            lastLocalRectMousePos = frameMousePos;
            currentColor = color;
            switch (state)
            {
                case State.Paint:
                    if (!overSideBar)
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            CallInstrument(lastLocalImageMousePos);
                        }
                    }
                    break;
                case State.Moving:
                    Move(frameMousePos);
                    break;
                default:
                    break;
            }
        }


        public void CallInstrument(YVector pos)
        {
            var instrument = instrumentsList.Find(x => x.Name == currentInstrument);

            if (instrument != null)
            {
                instrument.Call(pos);
            }
        }



        // A cache of all opacity values (0-255) scaled down to 0-1 for performance
        private readonly float[] _opacities = Enumerable.Range(0, 256)
                                              .Select(o => o / 255f)
                                              .ToArray();

        public Color AlphaComposite(Color c1, Color c2)
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

        public void Fill(YVector imagePixel, Color color)
        {
            if (bitmapImage != null)
            {

                changed = true;
                imagePixel /= scale;

                YVector pos = new YVector(imagePixel.X, imagePixel.Y);


                bitmapImage.Lock();

                if (CanDrawRect(pos))
                {
                    if (IsCanDraw(pos, color))
                    {
                        var startDelta = GetDeltaColors(GetWritableBitmapColor(pos), color);

                        FloodFill(pos, color, startDelta);
                    }
                }

                bitmapImage.Unlock();
            }
        }
        public bool IsCanDraw(YVector currentPos, Color fillColor)
        {
            var currentPixelColor = GetWritableBitmapColor(currentPos);
            if (currentPixelColor == fillColor)
            {
                return false;
            }
            return true;
        }
        public bool CanDrawRect(YVector pos)
        {
            if (pos.X >= 0 && pos.X < bitmapImage.PixelWidth)
            {
                if (pos.Y >= 0 && pos.Y < bitmapImage.PixelHeight)
                {
                    return true;
                }
            }
            return false;
        }

        private void FloodFill(YVector pt, Color replacementColor, float startDelta)
        {
            Stack<YVector> pixels = new Stack<YVector>();
            pixels.Push(pt);

            while (pixels.Count > 0 && pixels.Count < (bitmapImage.PixelWidth * bitmapImage.PixelHeight) * 2)
            {
                YVector a = pixels.Pop();
                if (CanDrawRect(a))//make sure we stay within bounds
                {
                    var pixelColor = GetWritableBitmapColor(a);
                    var currentDelta = GetDeltaColors(pixelColor, replacementColor);

                    if (pixelColor != replacementColor)
                    {
                        if (startDelta - currentDelta < fillPrecision)
                        {
                            DrawPixel(a, replacementColor);
                            pixels.Push(new YVector(a.X - 1, a.Y));
                            pixels.Push(new YVector(a.X + 1, a.Y));
                            pixels.Push(new YVector(a.X, a.Y - 1));
                            pixels.Push(new YVector(a.X, a.Y + 1));
                        }
                    }
                }
            }
            return;
        }



        public float GetDeltaColors(Color c1, Color c2)
        {
            var first = ((c1.R + c1.G + c1.B) / 3f) / 255;
            var second = ((c2.R + c2.G + c2.B) / 3f) / 255;

            if (c1.A != c2.A)
            {
                first = 0;
                second = 1;
            }

            return Math.Abs(first - second);
        }

        public Color GetWritableBitmapColor(YVector pos)
        {
            try
            {
                unsafe
                {
                    IntPtr pBackBuffer = bitmapImage.BackBuffer;

                    byte* pBuff = (byte*)pBackBuffer.ToPointer();

                    var b = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride)];
                    var g = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 1];
                    var r = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 2];
                    var a = pBuff[4 * pos.XInt + (pos.YInt * bitmapImage.BackBufferStride) + 3];


                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch (Exception)
            {
            }
            return Color.FromArgb(255, 255, 255, 255);
        }


        public void SetInstrument(string name)
        {
            currentInstrument = name;
        }

        public void Draw(YVector imagePixel, Color color)
        {
            if (bitmapImage != null)
            {
                changed = true;
                imagePixel /= scale;
                YVector pos = new YVector(imagePixel.X, imagePixel.Y);

                bitmapImage.Lock();
                var bitmap = window.CurrentBrush.BrushBitmapImageScaled;
                for (int x = -(int)bitmap.Width / 2; x < (bitmap.Width / 2) - 1; x++)
                {
                    for (int y = -(int)bitmap.Height / 2; y < (bitmap.Height / 2) - 1; y++)
                    {

                        var forPos = new YVector(x, y);

                        var posOnBrush = forPos + new YVector(bitmap.Width / 2, bitmap.Height / 2);
                        var pixColorBrush = window.CurrentBrush.BrushBitmapImageScaled.GetPixel(posOnBrush.XInt, posOnBrush.YInt);
                        if (pixColorBrush.A == 0) continue;

                        pos = imagePixel + forPos;
                        if (pos.X >= 0 && pos.X < bitmapImage.PixelWidth)
                        {
                            if (pos.Y >= 0 && pos.Y < bitmapImage.PixelHeight)
                            {
                                try
                                {
                                    // Reserve the back buffer for updates.
                                    unsafe
                                    {


                                        //ImageColor
                                        var imageColor = GetWritableBitmapColor(pos);

                                        if (imageColor == color)
                                        {
                                            continue;
                                        }

                                        //colorBrush
                                        var c0 = Color.FromArgb((byte)(pixColorBrush.A * (color.A / 255f)), color.R, color.G, color.B);


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

        public void ReducePos(YVector pixels)
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

        public void SetWritableImage(WriteableBitmap bitmapImage)
        {
            this.bitmapImage = bitmapImage;
            mainImage.Source = bitmapImage;
        }

        public WriteableBitmap GetBitMap()
        {
            if (bitmapImage != null)
            {
                bitmapImage = bitmapImage.Clone();
                mainImage.Source = bitmapImage;
                return bitmapImage.Clone();
            }
            return null;
        }

        public WriteableBitmap GetOrginalBitmap()
        {
            if (bitmapImage != null)
            {
                return bitmapImage;
            }
            return null;
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
            anim.Completed += (a, g) => { notAnimated = true; };
            mainImage.BeginAnimation(property, anim);

        }
    }
}

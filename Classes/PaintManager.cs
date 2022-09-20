using Paint.Classes;
using System;
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


        public void Draw(YVector imagePixel, Color color)
        {
            if (bitmapImage != null)
            {
                imagePixel /= scale;

                if (imagePixel.X >= 0 && imagePixel.X < bitmapImage.PixelWidth)
                {
                    if (imagePixel.Y >= 0 && imagePixel.Y < bitmapImage.PixelHeight)
                    {
                        DrawPixel(imagePixel, color);
                        //colors[] = new PixelColor(0, 0, 0, 255);

                        //PutPixels(bitmapImage, colors, 0, 0);
                        //UploadImage(ConvertWriteableBitmapToBitmapImage(bitmapImage), true);
                    }
                }
            }
        }
        public void DrawPixel(YVector pos, Color color)
        {
            int column = pos.XInt;
            int row = pos.YInt;

            try
            {
                // Reserve the back buffer for updates.
                bitmapImage.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = bitmapImage.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += row * bitmapImage.BackBufferStride;
                    pBackBuffer += column * 4;

                    // Compute the pixel's color.
                    int color_data = ((color.A << 24) + (color.R << 16) + (color.G << 8) + color.B);

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }

                // Specify the area of the bitmap that changed.
                bitmapImage.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bitmapImage.Unlock();
            }
        }

        public void Move(YVector frameMousePos)
        {
            mainImage.Margin = imagePos;
            var delta = startPos - frameMousePos;

            var newPos = new Thickness(imagePos.Left - delta.X, imagePos.Top - delta.Y, 0, 0);

            newPos.Left = Clamp(newPos.Left, -(mainImage.Width - 10), (window.ActualWidth) - 150);
            newPos.Top = Clamp(newPos.Top, -(mainImage.Height - 10), (window.ActualHeight) - 100);

            mainImage.Margin = newPos;
        }

        public void UploadImage(WriteableBitmap bitmapImage)
        {
            this.bitmapImage = bitmapImage;

            scale = 1f;
            zoomStep = 0.2f;

            initSize = new YVector(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            while (mainImage.Width > window.ActualWidth)
            {
                scale -= 0.01f;
                zoomStep -= 0.001f;
                mainImage.Width = initSize.X * scale;
                mainImage.Height = initSize.Y * scale;
            }
            while (mainImage.Width < window.ActualWidth + 100)
            {
                scale += 0.01f;
                zoomStep += 0.001f;
                mainImage.Width = initSize.X * scale;
                mainImage.Height = initSize.Y * scale;
            }
            imagePos.Left = 0;
            imagePos.Top -= mainImage.Height * 0.25f;

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
            if (state == State.Paint)
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


                    imagePos = new Thickness(imagePos.Left - (deltaVector.X / 2f), imagePos.Top - (deltaVector.Y / 2f), 0, 0);
                    mainImage.Margin = imagePos;
                    mainImage.Width = initSize.X * scale;
                    mainImage.Height = initSize.Y * scale;
                }
            }
        }


        public void Animation(float to, float from, DependencyProperty property)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = from;
            anim.To = to;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(350));
            mainImage.BeginAnimation(property, anim);

        }
    }
}

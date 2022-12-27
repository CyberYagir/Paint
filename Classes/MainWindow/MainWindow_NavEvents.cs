using Paint.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace Paint
{
    public partial class MainWindow
    {

        private void frame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PaintManager.IsChanged)
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
            var framePos = new YVector(e.GetPosition(GridFrame));
            PaintManager.SetStartPos(framePos);
            PaintManager.SetState(PaintManager.State.Moving);

        }
        private void frame_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            PaintManager.SetState(PaintManager.State.Paint);
        }
        public void AddAction()
        {
            UndoRendo.AddAction();
            UpdateUndoRendoDebug();
        }
        public void UpdateUndoRendoDebug()
        {
            return;
            //var list = UndoRendo.Bitmaps;
            //for (int i = 0; i < UndoRendoDebug.Children.Count; i++)
            //{
            //    if (i < list.Count)
            //    {
            //        (UndoRendoDebug.Children[i] as Image).Source = list[i];
            //    }
            //    else
            //    {
            //        (UndoRendoDebug.Children[i] as Image).Source = null;
            //    }
            //}
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
                var framePos = new YVector(e.GetPosition(GridFrame));
                var imagePos = new YVector(e.GetPosition(MainImage));
                PaintManager.Update(imagePos, framePos, ColorPicker.SelectedColor, e, isOverSidebar);
                var pos = new YVector(e.GetPosition(GridFrame));
                SetBrushImage(pos);
        }

        private void UpdateFill(YVector pos)
        {
            if (PaintManager.CurrentInstrument == "Fill")
            {
                FillImage.Width = 30;
                FillImage.Height = 30;
                FillImage.Margin = new Thickness((pos.X - FillImage.Width / 2f) + FillImage.Width, (pos.Y - FillImage.Height / 2f) + FillImage.Height, 0, 0);
            }
        }

        private void frame_MouseLeave(object sender, MouseEventArgs e)
        {
            PaintManager.SetState(PaintManager.State.Paint);
        }
        private void frame_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            PaintManager.Scale(e.Delta);
            SetBrushImage(new YVector(Mouse.GetPosition(GridFrame)));

        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == 0) return;
            var delta = new YVector(e.PreviousSize.Width - e.NewSize.Width, e.PreviousSize.Height - e.NewSize.Height);
            PaintManager.ReducePos(delta / 2f);
        }

        private void frame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var framePos = new YVector(e.GetPosition(GridFrame));
            var imagePos = new YVector(e.GetPosition(MainImage));
            PaintManager.Update(imagePos, framePos, ColorPicker.SelectedColor, e, isOverSidebar);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            UndoRendo.Undo();
            UpdateUndoRendoDebug();
        }
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            UndoRendo.Redo();
            UpdateUndoRendoDebug();
        }

        private void LeftPanelHolder_MouseEnter(object sender, MouseEventArgs e)
        {
            isOverSidebar = true;
        }

        private void LeftPanelHolder_MouseLeave(object sender, MouseEventArgs e)
        {
            isOverSidebar = false;
        }
    }
}

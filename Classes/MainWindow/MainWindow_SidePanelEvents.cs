using Paint.Classes;
using Paint.Forms;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Paint
{
    public partial class MainWindow
    {
        #region BrushBlock

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaintManager.CurrentInstrument != "Fill")
            {
                PaintManager.SetInstrument("Fill");
                UpdateFill(new YVector(Mouse.GetPosition(GridFrame)));
            }
            else
            {
                PaintManager.SetInstrument("Brush");
            }

            var isFillOn = PaintManager.CurrentInstrument == "Fill";

            FillIcon.Opacity = !isFillOn ? 0.2f : 1f;

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
                BrushesManager.ChangeBrushScale((float)BrushSizeSlider.Value);
                BrushImage.Width = CurrentBrush.BrushBitmapImageScaled.Width;
                BrushImage.Height = CurrentBrush.BrushBitmapImageScaled.Height;
            }
        }

        #endregion

        #region BrushBlock
        private void SelectBrushButton_Click(object sender, RoutedEventArgs e)
        {
            BrushesWindow brushesWindow = new BrushesWindow(BrushesManager, LocalSystem, this);
            brushesWindow.ShowDialog();
        }
        #endregion

        #region FillBlock
        private void FillDelta_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PaintManager != null)
            {
                PaintManager.SetFillPrecision((int)FillDelta.Value / 254f);
                FillDeltaText.Content = (int)FillDelta.Value;
            }
        }
        #endregion
    }
}

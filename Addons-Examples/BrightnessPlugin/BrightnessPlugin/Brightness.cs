using Paint;
using Paint.Classes.Instruments;

namespace BrightnessPlugin
{
    public class Brightness : Plugin
    {
        public override void Init(MainWindow mainWindow)
        {
            base.Init(mainWindow);
            name = "Brightness";
        }

        public override void Call()
        {
            base.Call();

            var brightnessWindow = new BrightnessWindow(window);
            brightnessWindow.ShowDialog();
        }
    }
}

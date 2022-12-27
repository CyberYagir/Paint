namespace Paint.Classes.Instruments
{
    class ToolFill : Tool
    {

        public override void Init(MainWindow mainWindow)
        {
            base.Init(mainWindow);
            name = "Fill";
        }

        public override void Call(YVector pos)
        {
            window.PaintManager.Fill(pos, window.PaintManager.CurrentColor);
        }
    }
}

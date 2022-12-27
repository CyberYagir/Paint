namespace Paint.Classes.Instruments
{
    class ToolBrush : Tool
    {

        public override void Init(MainWindow mainWindow)
        {
            base.Init(mainWindow);
            name = "Brush";
        }

        public override void Call(YVector pos)
        {
            window.PaintManager.Draw(pos, window.PaintManager.CurrentColor);
        }
    }
}

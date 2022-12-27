namespace Paint.Classes.Instruments
{
    public abstract class LoadableItem
    {
        protected string name;

        protected MainWindow window;


        public string Name => name;


        public virtual void Init(MainWindow mainWindow)
        {
            window = mainWindow;
        }
    }
}

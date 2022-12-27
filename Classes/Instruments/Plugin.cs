namespace Paint.Classes.Instruments
{
    public abstract class Plugin : LoadableItem, IPlugin
    {
        public virtual void Call()
        {
        }
        public virtual void AfterWindowLoaded()
        {

        }
    }
}

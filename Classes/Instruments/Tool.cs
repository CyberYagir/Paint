namespace Paint.Classes.Instruments
{
    public abstract class Tool : LoadableItem, ITool
    {
        public virtual void Call(YVector pos)
        {
        }
    }
}

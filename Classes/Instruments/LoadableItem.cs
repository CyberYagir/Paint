using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Paint.Classes.Instruments
{
    public abstract class Instrument: IPlugin
    {
        protected string instrumentName;

        protected MainWindow window;


        public string InstrumentName => instrumentName;


        public virtual void Init(MainWindow mainWindow)
        {
            window = mainWindow;
        }


        public virtual void Call(YVector pos)
        {
        }
    }
}

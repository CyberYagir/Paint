using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

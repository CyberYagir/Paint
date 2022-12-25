using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Paint.Classes
{
    public class HotKey
    {
        private ModifierKeys modifier;
        private Key key;
        private Action action;

        public ModifierKeys Modifier => modifier;
        public Key Key => key;

        public HotKey(ModifierKeys modifier, Key key, Action action)
        {
            this.modifier = modifier;
            this.key = key;
            this.action = action;
        }

        public void Fire()
        {
            action.Invoke();
        }
    }
    public class HotKeysManager
    {
        List<HotKey> hotKeys = new List<HotKey>();

        public void CheckHotKeys(KeyEventArgs e)
        {
            foreach (var item in hotKeys)
            {
                if (item.Key == e.Key && Keyboard.Modifiers == item.Modifier)
                {
                    item.Fire();
                    break;
                }
            }
        }

        public void AddHotKey(HotKey newHotKey)
        {
            hotKeys.Add(newHotKey);
        }
    }
}

using Paint.Classes;
using Paint.Forms;
using System.Windows;
using System.Windows.Input;

namespace Paint
{
    public partial class MainWindow
    {
        public void AddAllHotKeys()
        {
            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Z, delegate
            {
                UndoRendo.Undo();
            }));
            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control | ModifierKeys.Shift, Key.Z, delegate
            {
                UndoRendo.Redo();
            }));
            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Y, delegate
            {
                UndoRendo.Redo();
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.Y, delegate
            {
                UndoRendo.Redo();
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.P, delegate
            {
                OpenPlugins(PluginsWindow.DisplayState.All);
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.B, delegate
            {
                SelectBrushButton_Click(null, new RoutedEventArgs());
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.O, delegate
            {
                Createbtn_Click(null, new RoutedEventArgs());
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.N, delegate
            {
                OpenFile(null, new RoutedEventArgs());
            }));

            KeysManager.AddHotKey(new HotKey(ModifierKeys.Control, Key.S, delegate
            {
                Savebtn_Click(null, new RoutedEventArgs());
            }));
        }
    }
}

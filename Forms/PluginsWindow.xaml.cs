using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.Forms
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class PluginsWindow : Window
    {
        MainWindow window;
        DisplayState state = DisplayState.All;
        public enum DisplayState
        {
            All, Instruments, Plugins
        }

        public PluginsWindow(MainWindow mainWindow, DisplayState state)
        {
            InitializeComponent();
            this.state = state;
            window = mainWindow;


            UpdateList();
        }

        public void UpdateList()
        {
            listBox.Items.Clear();
            if (state == DisplayState.All || state == DisplayState.Instruments)
            {
                foreach (var item in window.InstrumentsLoader.LoadedData())
                {
                    CreateListItem($"File: {Path.GetFileNameWithoutExtension(item.File)}", item.File);
                    foreach (var instrument in item.Items)
                    {
                        CreateListItem($"          Tool:{instrument.InstrumentName}", item.File).IsEnabled = false; ;
                    }
                }
            }
        }

        public ListBoxItem CreateListItem(string text, string path)
        {
            var listItem = new ListBoxItem();
            listItem.Content = text;;
            listItem.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            listItem.FontSize = 16;
            listItem.MouseUp += (a, e) =>
            {
                string argument = "/select, \"" + Path.GetFullPath(path) + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            };
            listBox.Items.Add(listItem);

            return listItem;
        }


        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenPath(window.InstrumentsLoader.Folder);
        }

        public void OpenPath(string path)
        {
            Process.Start(new ProcessStartInfo(path));
        }

        private void Display_Click(object sender, RoutedEventArgs e)
        {
            Display.Content = "Display: ";
            switch (state)
            {
                case DisplayState.All:
                    state = DisplayState.Instruments;
                    Display.Content += "Tool";
                    break;
                case DisplayState.Instruments:
                    state = DisplayState.Plugins;
                    Display.Content += "Plugins";
                    break;
                case DisplayState.Plugins:
                    state = DisplayState.All;
                    Display.Content += "All";
                    break;
            }
            UpdateList();
        }
    }
}

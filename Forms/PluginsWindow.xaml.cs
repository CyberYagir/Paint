using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.Forms
{
    public partial class PluginsWindow : Window
    {
        MainWindow window;
        DisplayState state = DisplayState.All;

        string wihiteSpace = "          ";

        public enum DisplayState
        {
            All, Tools, Plugins
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


            CreateListItem($"Loaded: ", null);
            CreateListItem(wihiteSpace + $"Tools: {window.AddonsLoader.GetTools().LoadedData.Count}", null);
            CreateListItem(wihiteSpace + $"Plugins: {window.AddonsLoader.GetPlugins().LoadedData.Count}", null);

            if (state == DisplayState.All || state == DisplayState.Tools)
            {
                DrawTools();
            }
            if (state == DisplayState.All || state == DisplayState.Plugins)
            {
                DrawPlugins();
            }
        }

        public void DrawTools()
        {
            var items = window.AddonsLoader.GetTools().LoadedData;
            foreach (var item in items)
            {
                CreateListItem($"File: {Path.GetFileNameWithoutExtension(item.File)}", item.File);
                foreach (var tool in item.Items)
                {
                    CreateListItem(wihiteSpace + $"Tool: {tool.Name}", item.File).IsEnabled = false; ;
                }
            }
        }

        public void DrawPlugins()
        {
            var items = window.AddonsLoader.GetPlugins().LoadedData;
            foreach (var item in items)
            {
                CreateListItem($"File: {Path.GetFileNameWithoutExtension(item.File)}", item.File);
                foreach (var instrument in item.Items)
                {
                    CreateListItem(wihiteSpace + $"Plugin: {instrument.Name}", item.File).IsEnabled = false; ;
                }
            }
        }

        public ListBoxItem CreateListItem(string text, string path)
        {
            var listItem = new ListBoxItem();
            listItem.Content = text; ;
            listItem.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            listItem.FontSize = 16;
            listItem.MouseUp += (a, e) =>
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string argument = "/select, \"" + Path.GetFullPath(path) + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            };
            listBox.Items.Add(listItem);

            return listItem;
        }


        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenPath(window.AddonsLoader.Folder);
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
                    state = DisplayState.Tools;
                    Display.Content += "Tool";
                    break;
                case DisplayState.Tools:
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

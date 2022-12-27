using Paint.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paint
{
    public partial class MainWindow
    {
        public void OpenFile(object sender, RoutedEventArgs e) => MenuManager.OpenFile();
        private void OpenPlugins(PluginsWindow.DisplayState state)
        {
            PluginsWindow window = new PluginsWindow(this, state);
            window.ShowDialog();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Visibility = Visibility.Hidden;
        }
        private void Createbtn_Click(object sender, RoutedEventArgs e)
        {
            CreateAtlasWindow createWindow = new CreateAtlasWindow(this);
            createWindow.ShowDialog();
        }
        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            MenuManager.SaveFile(MainImage.Source);
        }

        private void OpenToolsBtn(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.Tools);
        }

        public void OpenPlugins(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.Plugins);
        }

        private void OpenAddons(object sender, RoutedEventArgs e)
        {
            OpenPlugins(PluginsWindow.DisplayState.All);
        }
        private void CreatorButton(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Creator: Yaroslav Khromov", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void GitHub(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/CyberYagir");
        }
    }
}

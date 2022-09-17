using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Paint.Forms
{
    /// <summary>
    /// Логика взаимодействия для CreateAtlas.xaml
    /// </summary>
    public partial class CreateAtlas : Window
    {
        public CreateAtlas()
        {
            InitializeComponent();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

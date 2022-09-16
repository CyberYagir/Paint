namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public static class FilterGenerator
    {
        public static string GenerateFilter(string name, params string[] formats)
        {
            string result = $"{name}|";
            if (formats.Length == 0)
            {
                return result + "*.*";
            }
            for (int i = 0; i < formats.Length; i++)
            {
                result += $"*.{formats[i]};";
            }
            return result;
        }
    }
}

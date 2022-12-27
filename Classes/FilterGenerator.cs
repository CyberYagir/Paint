namespace Paint
{
    public static class FilterGenerator
    {
        public static string GenerateFilter(string name, params string[] formats)
        {
            string result = $"";
            for (int i = 0; i < formats.Length; i++)
            {
                result += $"{formats[i]}|*.{formats[i]}|";
            }
            result += "*.*|*.*";
            return result;
        }
    }
}

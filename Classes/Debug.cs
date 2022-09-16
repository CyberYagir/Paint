using System.Windows;
namespace Paint
{
    public static class Debug
    {
        public static void Log(object obj)
        {
            MessageBox.Show(obj.ToString());
        }
    }
}

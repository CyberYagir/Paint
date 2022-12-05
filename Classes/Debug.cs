using System.Windows;
namespace Paint
{
    public static class Debug
    {
        public static void Log(object obj)
        {
            if (obj != null)
            {
                MessageBox.Show(obj.ToString());
            }
            else
            {
                MessageBox.Show("NULL");
            }
        }
    }
}

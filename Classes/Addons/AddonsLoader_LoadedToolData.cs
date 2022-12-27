using Paint.Classes.Instruments;
using System.Collections.Generic;

namespace Paint.Classes
{
    public partial class AddonsLoader
    {
        public class LoadedData<T>
        {
            private List<T> items = new List<T>();
            private string file;

            public LoadedData(List<T> item, string filePath)
            {
                items = item;
                file = filePath;
            }

            public List<T> Items => items;
            public string File => file;
        }
    }
}

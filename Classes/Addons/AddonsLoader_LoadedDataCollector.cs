using System.Collections.Generic;

namespace Paint.Classes
{
    public partial class AddonsLoader
    {
        public class LoadedDataCollector<T>
        {
            private List<T> allItems = new List<T>();
            private List<LoadedData<T>> loadedData = new List<LoadedData<T>>();


            public List<T> AllItems => allItems;
            public List<LoadedData<T>> LoadedData => loadedData;



            public void AddItem(List<T> items, string filePath)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    allItems.Add(items[i]);
                }
                loadedData.Add(new LoadedData<T>(items, filePath));
            }
        }
    }
}

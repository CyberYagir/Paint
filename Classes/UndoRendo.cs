using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Paint.Classes
{
    class UndoRendo
    {
        private int undoRendoLimit = 10;
        private List<WriteableBitmap> bitMaps = new List<WriteableBitmap>(10);
        private PaintManager paintManager;

        private int lastInList = 0;

        public List<WriteableBitmap> Bitmaps => bitMaps.ToList();

        public UndoRendo(PaintManager paintManager)
        {
            this.paintManager = paintManager;
            bitMaps = new List<WriteableBitmap>(undoRendoLimit);
        }

        public void AddAction()
        {
            if (bitMaps.Count + 1 >= undoRendoLimit)
            {
                bitMaps.RemoveAt(0);
            }

            while (lastInList > bitMaps.Count)
            {
                bitMaps.RemoveAt(bitMaps.Count-1);
            }

            bitMaps.Add(paintManager.GetBitMap());
            lastInList = bitMaps.Count;
        }

        public void Undo()
        {
            if (bitMaps.Count > 1)
            {
                var last = bitMaps[bitMaps.Count - 2];
                paintManager.SetWritableImage(last.Clone());
                bitMaps.RemoveAt(bitMaps.Count - 1);
            }
        }

        public void Rendo()
        {

        }

        public void Clear()
        {
            bitMaps.Clear();
        }
    }
}

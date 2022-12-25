using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Paint.Classes
{
    class UndoRendo
    {
        private int undoRendoLimit = 20;
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

            while (lastInList < bitMaps.Count - 1)
            {
                bitMaps.RemoveAt(bitMaps.Count - 1);
            }

            bitMaps.Add(paintManager.GetBitMap());
            lastInList = bitMaps.Count - 1;
        }

        public void Undo()
        {
            if (bitMaps.Count > 1)
            {
                lastInList--;
                try
                {
                    var last = bitMaps[lastInList];
                    paintManager.SetWritableImage(last.Clone());
                }
                catch (Exception)
                {
                }
                //bitMaps.RemoveAt(bitMaps.Count - 1);
            }
        }

        public void Redo()
        {
            lastInList++;
            if (lastInList > bitMaps.Count - 1)
            {
                lastInList = bitMaps.Count - 1;
            }

            var last = bitMaps[lastInList];
            paintManager.SetWritableImage(last.Clone());
        }

        public void Clear()
        {
            bitMaps.Clear();
        }
    }
}

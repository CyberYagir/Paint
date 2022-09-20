using System.Collections.Generic;

namespace Paint
{
    public sealed class ArrayConverter<T>
    {
        public T[,] TwoDimesional(List<T> list, int width, int height)
        {
            T[,] twoDimensional = new T[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    twoDimensional[i, k] = list[i * height + k];
                }
            }

            return twoDimensional;
        }
    }
}

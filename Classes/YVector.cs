using System.Windows;

namespace Paint.Classes
{
    public class YVector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public int XInt => (int)X;
        public int YInt => (int)Y;


        public float XFloat => (float)X;
        public float YFloat => (float)Y;

        public double Mult => X * Y;


        public Vector Vector => new Vector(X, Y);


        public YVector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public YVector(int x, int y)
        {
            X = x;
            Y = y;
        }
        public YVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public YVector(Vector vec)
        {
            X = vec.X;
            Y = vec.Y;
        }
        public YVector(Point vec)
        {
            X = vec.X;
            Y = vec.Y;
        }

        public YVector()
        {
        }

        public static YVector operator +(YVector a, YVector b)
        {
            return new YVector(a.Vector + b.Vector);
        }

        public static YVector operator -(YVector a, YVector b)
        {
            return new YVector(a.Vector - b.Vector);
        }
        public static YVector operator /(YVector a, float b)
        {
            return new YVector(a.Vector/b);
        }

        public static YVector operator *(YVector a, float b)
        {
            return new YVector(a.Vector * b);
        }
    }
}

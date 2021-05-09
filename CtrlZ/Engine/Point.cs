using System;

namespace CtrlZ
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point() : this(0, 0) { }

        public void Zero()
        {
            X = 0;
            Y = 0;
        }

        public static implicit operator Point(System.Drawing.PointF systemPoint)
        {
            return new Point((int)systemPoint.X, (int)systemPoint.Y);
        }

        public static implicit operator System.Drawing.PointF(Point point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }
        public static implicit operator Point(System.Drawing.Point point)
        {
            return new Point(point.X, point.Y);
        }

        public static implicit operator System.Drawing.Point(Point point)
        {
            return new System.Drawing.Point((int) point.X, (int) point.Y);
        }

        public static Point operator -(Point point) =>
            new Point(-point.X, point.Y);

        public static Point operator +(Point point1, Point point2) =>
            new Point(point1.X + point2.X, point1.Y + point2.Y);

        public static Point operator -(Point point1, Point point2) =>
            point1 + -point2;

        public static Point operator /(Point point, int number) =>
            new Point(point.X / number, point.Y / number);

        public static Point operator *(Point point, int number) =>
            new Point(point.X * number, point.Y * number);

        public override string ToString() =>
            $"({X}, {Y})";
    }
}
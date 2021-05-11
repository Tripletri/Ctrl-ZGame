using System;
using System.Drawing;

namespace RobotVoyager.Engine
{
    internal class PointF
    {
        public float X { get; set; }
        public float Y { get; set; }

        public double Length => Math.Sqrt(X * X + Y * Y);

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public PointF() : this(0, 0) { }

        public void Zero()
        {
            X = 0;
            Y = 0;
        }

        public PointF Normalize()
        {
            return new PointF(X / (float) Length, Y / (float) Length);
        }

        public static implicit operator PointF(System.Drawing.PointF systemPoint)
        {
            return new PointF(systemPoint.X, systemPoint.Y);
        }

        public static implicit operator System.Drawing.PointF(PointF point)
        {
            return new System.Drawing.PointF(point.X, point.Y);
        }

        public static implicit operator PointF(Point point)
        {
            return new PointF(point.X, point.Y);
        }

        public static implicit operator Point(PointF point)
        {
            return new Point((int) point.X, (int) point.Y);
        }

        public static PointF operator -(PointF point)
        {
            return new PointF(-point.X, -point.Y);
        }

        public static PointF operator +(PointF point1, PointF point2)
        {
            return new PointF(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static PointF operator -(PointF point1, PointF point2)
        {
            return point1 + -point2;
        }

        public static PointF operator /(PointF point, float number)
        {
            return new PointF(point.X / number, point.Y / number);
        }

        public static PointF operator *(PointF point, float number)
        {
            return new PointF(point.X * number, point.Y * number);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
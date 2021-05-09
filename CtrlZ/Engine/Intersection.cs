using System;
using System.Drawing;

namespace CtrlZ.Engine
{
    public static class Intersection
    {
        public enum IntersectionType
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public static IntersectionType GetIntersectionType(RectangleF first, RectangleF second)
        {
            var intersection = RectangleF.Intersect(first, second);
            if (intersection.Width > intersection.Height)
            {
                //on top/bottom
                var distToTop = Math.Abs(intersection.Top - first.Top);
                var distToBottom = Math.Abs(intersection.Bottom - first.Bottom);
                return distToTop > distToBottom ? IntersectionType.Top : IntersectionType.Bottom;
                
            }
            else
            {
                //on right/left
                var distToRight = Math.Abs(intersection.Right - first.Right);
                var distToLeft = Math.Abs(intersection.Left - first.Left);

                return distToRight < distToLeft ? IntersectionType.Left : IntersectionType.Right;
            }
        }
    }
}
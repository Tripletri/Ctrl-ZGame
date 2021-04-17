using System.Drawing;

namespace CtrlZ
{
    internal abstract class Transform
    {
        public PointF Position;

        protected Transform(PointF position)
        {
            Position = position;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ
{
    class Box : Square, ITimeTraveler
    {
        private List<PointF> pastPositions = new List<PointF>();

        public Box(Rectangle rectangle) : base(rectangle, new Bitmap("sprites/Objects/Box.png"))
        {
            Static = false;
            OnStateUpdate += BoxOnStateUpdate;    
        }

        private void BoxOnStateUpdate()
        {
            pastPositions.Add(Position);
        }

        public Rectangle GoBackInTime(int time)
        {
            Position = pastPositions.First();
            pastPositions = new List<PointF>();
            return Rectangle.Empty;
        }
    }
}

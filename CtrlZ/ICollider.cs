using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ
{
    interface ICollider
    {
        RectangleF CollideRectangle { get; }
        //bool Collision { get; set; }
        //bool Static { get; set; } 
        PointF Velocity { get; set; }
        void MovePhysics(IReadOnlyCollection<ICollider> colliders);
        void Move(IReadOnlyCollection<ICollider> colliders);
    }
}

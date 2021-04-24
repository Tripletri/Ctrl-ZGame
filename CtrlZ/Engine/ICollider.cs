using System;
using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ
{
    interface ICollider
    {
        RectangleF CollideRectangle { get; }
        RectangleF ColliderArea { get; }
        bool Collision { get; set; }
        bool Static { get; set; } 
        PointF Velocity { get; set; }
        void MovePhysics(IReadOnlyCollection<ICollider> colliders);
        void Move(IReadOnlyCollection<ICollider> colliders);

         event Action<ICollider> OnCollision;
         event Action<ICollider> OnTrigger;
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ.Engine
{
    internal interface ICollider
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

        void InvokeTrigger(ICollider other);

        bool MovableStatic { get; set; }
    }
}
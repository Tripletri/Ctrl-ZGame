using System;
using System.Collections.Generic;
using System.Drawing;

namespace RobotVoyager.Engine
{
    internal interface ICollider
    {
        RectangleF CollideRectangle { get; }
        RectangleF ColliderArea { get; }
        bool Collision { get; set; }
        bool Static { get; set; }
        PointF Velocity { get; set; }

        bool MovedThisFrame { get; set; }
        bool MovedPhysicsThisFrame { get; set; }

        bool MovableStatic { get; set; }
        void MovePhysics(IReadOnlyCollection<ICollider> colliders);
        void Move(IReadOnlyCollection<ICollider> colliders);

        event Action<ICollider> OnCollision;
        event Action<ICollider> OnTrigger;

        void InvokeTrigger(ICollider other);
        void InvokeCollision(ICollider other);
    }
}
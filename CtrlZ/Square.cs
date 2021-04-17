using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ
{
    class Square : Transform, ICollider, IPhysics, ISprite
    {
        public RectangleF ColliderArea { get; set; }

        public RectangleF CollideRectangle =>
            new RectangleF(Position.X + ColliderArea.X, Position.Y + ColliderArea.Y, ColliderArea.Width,
                ColliderArea.Height);

        private Sprite sprite;
        public bool Collision { get; set; } = true;
        public bool Static { get; set; } = true;
        public float FrictionCoefficient { get; set; } = 1;
        public PointF SlidingFrictionForce { get; private set; } = new PointF();
        public PointF GravityForce => new PointF(0, Gravity * Mass);
        public float Gravity { get; } = 0.4f;
        public float Mass { get; set; } = 1;
        public PointF Acceleration
        {
            get
            {
                var deltaVelocity = new PointF();
                deltaVelocity += SlidingFrictionForce;
                deltaVelocity += GravityForce;
                return deltaVelocity / Mass;
            }
        }

        private PointF velocity = new PointF();
        public PointF Velocity
        {
            get => Static ? new PointF(0, 0) : velocity;
            set => velocity = value;
        }

        public event Action<ICollider> OnCollision;

        private RectangleF PredictCollider(ICollider collider)
        {
            var nextCollider = collider.CollideRectangle;

            if (Velocity.X < 0)
                nextCollider.X += collider.Velocity.X;
            else if (Velocity.X > 0)
                nextCollider.Width += collider.Velocity.X;

            if (Velocity.Y < 0)
                nextCollider.Y += collider.Velocity.Y;
            else if (Velocity.Y > 0)
                nextCollider.Height += collider.Velocity.Y;

            return nextCollider;
        }

        public bool IsIntersectsWith(ICollider other)
        {
            return !(other == this || !PredictCollider(this).IntersectsWith(other.CollideRectangle));
        }

        public RectangleF ResolveCollision(ICollider other)
        {
            OnCollision?.Invoke(other);
            if (Static || other == this || !other.CollideRectangle.IntersectsWith(CollideRectangle))
                return RectangleF.Empty;

            var intersection = RectangleF.Intersect(CollideRectangle, other.CollideRectangle);
            if (intersection.Width > intersection.Height)
            {
                //on top/bottom
                if (intersection.Top == other.CollideRectangle.Top)
                    Position.Y = other.CollideRectangle.Top - CollideRectangle.Height;
                if (intersection.Bottom == other.CollideRectangle.Bottom)
                    Position.Y = other.CollideRectangle.Bottom;
                Velocity.Y = 0;
            }
            else
            {
                //on right/left
                if (intersection.Left == other.CollideRectangle.Left)
                    Position.X = other.CollideRectangle.Left - CollideRectangle.Width;
                if (intersection.Right == other.CollideRectangle.Right)
                    Position.X = other.CollideRectangle.Right;
            }
            return intersection;
        }

        private void ProceedPhysics(IPhysics physics, ICollider collider)
        {
            var frictionCoefficient = FrictionCoefficient;
            frictionCoefficient += physics.FrictionCoefficient;
            frictionCoefficient /= 2;
            var avgSpeed = (collider.Velocity + Velocity) / 2;
            var intersection = RectangleF.Intersect(CollideRectangle, collider.CollideRectangle);
            if (intersection.Width > intersection.Height)
            {
                if (this is Player)
                    return;
                //on top/bottom
                if (Math.Abs(Velocity.X) < 0.0001)
                {
                    Velocity.X = 0;
                    SlidingFrictionForce.Zero();
                    return;
                }
                SlidingFrictionForce =
                    new PointF(-frictionCoefficient * Mass * Gravity * (Math.Abs(Velocity.X) / Velocity.X), 0);
                if (Math.Abs(Velocity.X) - Math.Abs(SlidingFrictionForce.X) <= 0)
                    Velocity.X = 0;
            }
            else
            {
                //on right/left
                collider.Velocity.X = avgSpeed.X;
            }
        }

        public void MovePhysics(IReadOnlyCollection<ICollider> colliders)
        {
            if (Static)
                return;

            Velocity += Acceleration;

            foreach (var collider in colliders.Where(IsIntersectsWith))
            {
                if (collider is IPhysics physics)
                    ProceedPhysics(physics, collider);
            }
        }

        public void Move(IReadOnlyCollection<ICollider> colliders)
        {
            if (Static)
                return;
            Position += Velocity;

            var intersected = false;
            foreach (var collider in colliders)
            {
                var intersection = ResolveCollision(collider);
                if (!intersection.IsEmpty)
                    intersected = true;
            }

            if (!intersected)
                SlidingFrictionForce.Zero();
        }

        public Square(RectangleF rectangle) : base(rectangle.Location)
        {
            ColliderArea = new RectangleF(0, 0, rectangle.Width, rectangle.Height);
            sprite = new Sprite(rectangle);
        }

        public Square(RectangleF rectangle, Bitmap bmp) : this(rectangle)
        {
            sprite = new Sprite(rectangle, bmp);
        }

        public Square(Rectangle rectangle, AnimatedSprite animatedSprite): this(rectangle)
        {
            sprite = animatedSprite;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static CtrlZ.Engine.Intersection;

namespace CtrlZ.Engine
{
    internal class Square : Transform, ICollider, IPhysics, ISprite
    {
        protected Sprite Sprite;

        private PointF velocity = new PointF();

        public Square(Rectangle rectangle) : base(rectangle.Location)
        {
            ColliderArea = new RectangleF(0, 0, rectangle.Width, rectangle.Height);
            Sprite = new Sprite(RectangleF.Empty, new Bitmap(1, 1));
        }

        public Square(Rectangle rectangle, Bitmap bmp) : this(rectangle)
        {
            Sprite = new Sprite(rectangle, bmp);
        }

        public Square(Rectangle rectangle, Sprite animatedSprite) : this(rectangle)
        {
            Sprite = animatedSprite;
        }

        public Square(Rectangle rectangle, Animator animator, int delay, List<Image> images) : this(rectangle,
            new AnimatedSprite(animator, rectangle, delay, images)) { }

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

        public event Action<ICollider> OnCollision;
        public event Action<ICollider> OnTrigger;

        public void InvokeTrigger(ICollider other)
        {
            OnTrigger?.Invoke(other);
        }

        public bool MovableStatic { get; set; }

        public bool Static { get; set; } = true;
        public RectangleF ColliderArea { get; set; }

        public RectangleF CollideRectangle =>
            new RectangleF(Position.X + ColliderArea.X,
                Position.Y + ColliderArea.Y,
                ColliderArea.Width,
                ColliderArea.Height);

        public bool Collision { get; set; } = true;
        public PointF Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        public void MovePhysics(IReadOnlyCollection<ICollider> colliders)
        {
            if (Static)
                return;
            Velocity += Acceleration;
            foreach (var collider in colliders.Where(IsIntersectsWith))
                if (collider is IPhysics)
                    ProceedPhysics(collider);
        }

        public void Move(IReadOnlyCollection<ICollider> colliders)
        {
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

        public float FrictionCoefficient { get; set; } = 1;
        public bool CalculateFriction { get; set; } = true;

        public Sprite GetSprite()
        {
            return Sprite;
        }

        public void SetY(float y)
        {
            Position.Y = y - ColliderArea.Y;
        }

        public void SetX(float x)
        {
            Position.X = x - ColliderArea.X;
        }

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
            if (other == this || !other.CollideRectangle.IntersectsWith(CollideRectangle))
                return RectangleF.Empty;

            OnTrigger?.Invoke(other);
            other.InvokeTrigger(this);
            if (Static || !Collision || !other.Collision)
                return RectangleF.Empty;
            OnCollision?.Invoke(other);

            var intersection = RectangleF.Intersect(CollideRectangle, other.CollideRectangle);
            var intersectionType = GetIntersectionType(CollideRectangle, other.CollideRectangle);

            if ((intersectionType == IntersectionType.Right
                 || intersectionType == IntersectionType.Left) && other.Static)
                Velocity.X = 0;
            switch (intersectionType)
            {
                case IntersectionType.Left:
                    SetX(other.CollideRectangle.Left - CollideRectangle.Width);
                    break;
                case IntersectionType.Right:
                    SetX(other.CollideRectangle.Right);
                    break;
                case IntersectionType.Top:
                    SetY(other.CollideRectangle.Top - CollideRectangle.Height + other.Velocity.Y);
                    if (velocity.Y > 0)
                        Velocity.Y = 0;
                    break;
                case IntersectionType.Bottom:
                    SetY(other.CollideRectangle.Bottom);
                    if (Velocity.Y < 0)
                        Velocity.Y = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return intersection;
        }

        private void ProceedPhysics(ICollider collider)
        {
            var avgSpeed = (collider.Velocity + Velocity) / 2;
            if (collider.Static || Static)
                avgSpeed.Zero();
            if (collider.MovableStatic)
                avgSpeed = collider.Velocity;

            var intersection = RectangleF.Intersect(PredictCollider(this), collider.CollideRectangle);
            if (Math.Abs(intersection.Height - intersection.Width) < 5)
                return;
            var intersectionType = GetIntersectionType(PredictCollider(this), collider.CollideRectangle);
            if (intersectionType == IntersectionType.Bottom || intersectionType == IntersectionType.Top)
                
            {
                //ProceedFriction(physics);
                if (Math.Abs(Velocity.X) > 0.1)
                    Velocity.X -= 0.1f * (Velocity.X / Math.Abs(Velocity.X));
                else
                    Velocity.X = 0;
            }
            else
                collider.Velocity = new PointF(avgSpeed.X, collider.Velocity.Y);
        }

        //Deprecated
        private void ProceedFriction(IPhysics physics)
        {
            if (!physics.CalculateFriction || !CalculateFriction)
                return;
            var frictionCoefficient = (FrictionCoefficient + physics.FrictionCoefficient) / 2;
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
    }
}
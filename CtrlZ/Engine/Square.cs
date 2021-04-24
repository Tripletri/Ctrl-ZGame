using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ
{
    internal class Square : Transform, ICollider, IPhysics, ISprite
    {
        protected Sprite Sprite;

        private PointF velocity = new PointF();

        public event Action<ICollider> OnCollision;
        public event Action<ICollider> OnTrigger;

        public Square(Rectangle rectangle) : base(rectangle.Location)
        {
            ColliderArea = new RectangleF(0, 0, rectangle.Width, rectangle.Height);
            Sprite = new Sprite(RectangleF.Empty, new Bitmap(1, 1));
        }

        public Square(Rectangle rectangle, Bitmap bmp) : this(rectangle)
        {
            Sprite = new Sprite(rectangle, bmp);
        }

        public Square(Rectangle rectangle, AnimatedSprite animatedSprite) : this(rectangle)
        {
            Sprite = animatedSprite;
        }

        public Square(Rectangle rectangle, Animator animator, int delay, List<Image> images) : this(rectangle,
            new AnimatedSprite(animator, rectangle, delay, images)) { }

        public bool Static { get; set; } = true;
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
        public RectangleF ColliderArea { get; set; }

        public RectangleF CollideRectangle =>
            new RectangleF(Position.X + ColliderArea.X,
                Position.Y + ColliderArea.Y,
                ColliderArea.Width,
                ColliderArea.Height);

        public float FrictionCoefficient { get; set; } = 1;
        public bool CalculateFriction { get; set; } = true;

        public bool Collision { get; set; } = true;
        public PointF Velocity
        {
            get => Static ? new PointF(0, 0) : velocity;
            set => velocity = value;
        }

        public void MovePhysics(IReadOnlyCollection<ICollider> colliders)
        {
            if (Static)
                return;
            Velocity += Acceleration;
            foreach (var collider in colliders.Where(IsIntersectsWith))
                if (collider is IPhysics physics)
                    ProceedPhysics(physics, collider);
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
            if (Static || !Collision || !other.Collision)
                return RectangleF.Empty;
            OnCollision?.Invoke(other);

            var intersection = RectangleF.Intersect(CollideRectangle, other.CollideRectangle);
            if (intersection.Width > intersection.Height)
            {
                //on top/bottom
                if (intersection.Top == other.CollideRectangle.Top)
                {
                    //on top
                    SetY(other.CollideRectangle.Top - CollideRectangle.Height);
                    if (velocity.Y > 0)
                        Velocity.Y = 0;
                }
                else if (intersection.Bottom == other.CollideRectangle.Bottom)
                {
                    //on bottom
                    SetY(other.CollideRectangle.Bottom);
                    Velocity.Y = 0;
                }
            }
            else
            {
                //on right/left
                var distToRight = Math.Abs(intersection.Right - CollideRectangle.Right);
                var distToLeft = Math.Abs(intersection.Left - CollideRectangle.Left);
                if (distToRight < distToLeft) //lefter
                    SetX(other.CollideRectangle.Left - CollideRectangle.Width);
                else //righter
                    SetX(other.CollideRectangle.Right);
            }
            return intersection;
        }

        private void ProceedPhysics(IPhysics physics, ICollider collider)
        {
            var avgSpeed = (collider.Velocity + Velocity) / 2;
            var intersection = RectangleF.Intersect(PredictCollider(this), collider.CollideRectangle);

            if (Math.Abs(intersection.Height - intersection.Width) < 5)
                return;
            if (intersection.Width > intersection.Height)
                //on top/bottom
                ProceedFriction(physics);
            else
                //on right/left
                collider.Velocity.X = avgSpeed.X;
        }

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
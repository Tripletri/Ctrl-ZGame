﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static RobotVoyager.Engine.Intersection;

namespace RobotVoyager.Engine
{
    internal class Square : Transform, ICollider, IPhysics, ISprite
    {
        private IReadOnlyCollection<ICollider> colliders;
        protected Sprite Sprite;

        public PointF GravityForce => new PointF(0, 0.4f);
        public PointF Acceleration
        {
            get
            {
                var deltaVelocity = new PointF();
                deltaVelocity += GravityForce;
                return deltaVelocity;
            }
        }

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

        public event Action<ICollider> OnCollision;
        public event Action<ICollider> OnTrigger;

        public void InvokeTrigger(ICollider other)
        {
            OnTrigger?.Invoke(other);
        }

        public void InvokeCollision(ICollider other)
        {
            OnCollision?.Invoke(other);
        }

        public bool MovedPhysicsThisFrame { get; set; }
        public bool MovableStatic { get; set; }

        public bool Static { get; set; } = true;
        public RectangleF ColliderArea { get; set; }

        public RectangleF CollideRectangle =>
            new RectangleF(Position.X + ColliderArea.X,
                Position.Y + ColliderArea.Y,
                ColliderArea.Width,
                ColliderArea.Height);

        public bool Collision { get; set; } = true;
        public PointF Velocity { get; set; } = new PointF();
        public bool MovedThisFrame { get; set; }

        public void MovePhysics(IReadOnlyCollection<ICollider> colliders)
        {
            if (MovedPhysicsThisFrame)
                return;
            this.colliders = colliders;
            if (Static)
                return;
            Velocity += Acceleration;
            foreach (var collider in colliders.Where(IsIntersectsWith))
                if (collider is IPhysics)
                    ProceedPhysics(collider);
            MovedPhysicsThisFrame = true;
        }

        public void Move(IReadOnlyCollection<ICollider> colliders)
        {
            if (MovedThisFrame)
                return;
            this.colliders = colliders;
            Position += Velocity;
            foreach (var collider in colliders)
                ResolveCollision(collider);
            MovedThisFrame = true;
        }

        public float FrictionCoefficient { get; set; } = 1;
        public float DecelerationSpeed { get; set; }

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

        public void ResolveCollision(ICollider other)
        {
            if (other == this || !other.CollideRectangle.IntersectsWith(CollideRectangle))
                return;

            OnTrigger?.Invoke(other);
            other.InvokeTrigger(this);
            if (Static || !Collision || !other.Collision)
                return;
            OnCollision?.Invoke(other);
            other.InvokeCollision(this);

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
                    //Position.Y = other.CollideRectangle.Top - CollideRectangle.Height;
                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                    break;
                case IntersectionType.Bottom:
                    SetY(other.CollideRectangle.Bottom);
                    if (Velocity.Y < 0)
                        Velocity.Y = 0;
                    break;
            }
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
            if (intersectionType == IntersectionType.Top)
            {
                if (!collider.MovableStatic)
                {
                    collider.MovePhysics(colliders);
                    collider.Move(colliders);
                }
                ProceedFriction(collider);
            }
            else if (intersectionType != IntersectionType.Bottom)
            {
                collider.Velocity = new PointF(avgSpeed.X, collider.Velocity.Y);
            }
        }

        private void ProceedFriction(ICollider collider)
        {
            var coefficient = FrictionCoefficient;
            if (collider is IPhysics physics)
            {
                coefficient += physics.FrictionCoefficient;
                coefficient /= 2;
            }

            if (Math.Abs(Velocity.X) > DecelerationSpeed)
                Velocity.X -= DecelerationSpeed * (Velocity.X / Math.Abs(Velocity.X));
            else
                Velocity.X = 0;
            Position.X += collider.Velocity.X * coefficient;
        }
    }
}
using System.Collections.Generic;
using System.Drawing;
using CtrlZ.Engine;
using static CtrlZ.Engine.Intersection;

namespace CtrlZ
{
    internal class Player : Square
    {
        private const float maxSpeed = 3;
        private const float acceleration = 2f;
        private readonly AnimatedSprite idleAnimation;
        private readonly AnimatedSprite runAnimation;
        private bool isOnFloor;
        private int move;
        public bool Alive { get; private set; } = true;
        public string DeathReason { get; private set; }

        public Player(Rectangle rectangle, Animator animator) : base(rectangle)
        {
            OnCollision += PlayerOnCollision;
            CalculateFriction = false;
            Static = false;
            OnStateUpdate += PlayerOnStateUpdate;

            #region Animations

            runAnimation = new AnimatedSprite(animator, rectangle, 3,
                CreateImagesFromFolder("sprites/main/robot/images/robot_", 8), 1);

            idleAnimation =
                new AnimatedSprite(animator, rectangle, 7,
                    new List<Image> { new Bitmap("sprites/main/robot/images/robot_" + "01.png") }, 1);

            #endregion

            Sprite = idleAnimation;
        }

        private List<Image> CreateImagesFromFolder(string path, int count)
        {
            var images = new List<Image>();
            for (var i = 1; i <= count; i++)
            {
                var a = path + $"{i:00}.png";
                images.Add(new Bitmap(a));
            }
            return images;
        }

        private void PlayerOnStateUpdate()
        {
            if (!Alive)
                return;
            if (move == 0)
            {
                Sprite = idleAnimation;
                Velocity.X = 0;
                return;
            }
            Sprite.ReversY = move < 0;
            Sprite = runAnimation;
            Velocity.X += move * acceleration;

            if (Velocity.X > maxSpeed)
                Velocity.X = maxSpeed;
            if (Velocity.X < -maxSpeed)
                Velocity.X = -maxSpeed;
            move = 0;
        }

        private void PlayerOnCollision(ICollider collider)
        {
            var intersectionType = GetIntersectionType(CollideRectangle, collider.CollideRectangle);
            if (collider is Box && collider.Velocity.Y > 0 && intersectionType == IntersectionType.Bottom)
                Die("Smashed up");
            if (isOnFloor)
                return;
            if (intersectionType == IntersectionType.Top && Velocity.Y > 0)
                isOnFloor = true;
        }

        public void Move(int x)
        {
            move += x;
        }

        public void Jump()
        {
            if (!isOnFloor)
                return;
            Velocity.Y = -10;
            isOnFloor = false;
        }

        public void Die(string reason)
        {
            GetSprite().Visibility = false;
            DeathReason = reason;
            Alive = false;
            Collision = false;
        }
    }
}
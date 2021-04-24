using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ
{
    class Player : Square
    {
        private readonly AnimatedSprite runAnimation;
        private readonly AnimatedSprite idleAnimation;

        public Player(Rectangle rectangle, Animator animator) : base(rectangle)
        {
            OnCollision += PlayerOnCollision;
            CalculateFriction = false;
            var path = "sprites/main/images/Zapper_";
            runAnimation = new AnimatedSprite(animator, rectangle, 8, new List<Image>()
            {
                new Bitmap(path + "01.png"),
                new Bitmap(path + "02.png"),
                new Bitmap(path + "03.png"),
                new Bitmap(path + "04.png"),
                new Bitmap(path + "05.png"),
                new Bitmap(path + "06.png"),
            }, 1);
            idleAnimation =
                new AnimatedSprite(animator, rectangle, 15, new List<Image>() { new Bitmap(path + "06.png") }, 1);
            Sprite = idleAnimation;
        }

        private void PlayerOnCollision(ICollider collider)
        {
            if (isOnFloor)
                return;
            var intersection = RectangleF.Intersect(CollideRectangle, collider.CollideRectangle);
            if (intersection.Width > intersection.Height && intersection.Y > CollideRectangle.Y && Velocity.Y > 0)
                isOnFloor = true;
        }

        private const float maxSpeed = 3;
        private bool isOnFloor;
        private const float acceleration = 2f;

        public void Move(int x)
        {
            if (x == 0)
            {
                Sprite = idleAnimation;
                Velocity.X = 0;
                return;
            }
            Sprite.ReversY = x < 0;
            Sprite = runAnimation;
            Velocity.X += x * acceleration;

            if (Velocity.X > maxSpeed)
                Velocity.X = maxSpeed;
            if (Velocity.X < -maxSpeed)
                Velocity.X = -maxSpeed;
        }

        public void Jump()
        {
            if (!isOnFloor)
                return;
            Velocity.Y = -10;
            isOnFloor = false;
        }

        public void Die()
        {
            Position = new PointF(0, 0);
        }
    }
}
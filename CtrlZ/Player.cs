using System.Drawing;

namespace CtrlZ
{
    class Player : Square
    {
        public Player(Rectangle rectangle) : base(rectangle)
        {
            OnCollision += Player_OnCollision;
        }

       

        private void Player_OnCollision(ICollider collider)
        {
            var intersection = RectangleF.Intersect(CollideRectangle, collider.CollideRectangle);
            if (intersection.Width > intersection.Height && intersection.Y>CollideRectangle.Y)
                isOnFloor = true;
        }


        private float maxSpeed = 3;
        private bool isOnFloor = false;
        public void Move(int x)
        {
            if (x==0)
            {
                Velocity.X = 0;
            }
            Velocity.X += x * 2f;
            if (Velocity.X > maxSpeed)
                Velocity.X = maxSpeed;
            if (Velocity.X < -maxSpeed)
                Velocity.X = -maxSpeed;
        }

        public void Jump()
        {
            if(!isOnFloor)
                return;
            Velocity.Y -= 5;
            isOnFloor = false;
        }
    }
}

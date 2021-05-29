using System.Drawing;
using System.Linq;
using RobotVoyager.Engine;
using PointF = RobotVoyager.Engine.PointF;

namespace RobotVoyager.GameObjects
{
    internal class MovingPlatform : Platform
    {
        private bool movingForward = true;

        public MovingPlatform(PointF startPosition, PointF endPosition, float velocity, int length) : base(
            startPosition, length)
        {
            var headSquare = squares.First();
            var delta = endPosition - startPosition;
            var moveVector = delta.Normalize() * velocity;
            if (startPosition.Y > endPosition.Y)
                startPosition.Y += headSquare.CollideRectangle.Height;
            else if (startPosition.Y < endPosition.Y)
                endPosition.Y += headSquare.CollideRectangle.Height;
            if (startPosition.X > endPosition.X)
            {
                startPosition.X += headSquare.CollideRectangle.Width;
                endPosition.X -= headSquare.CollideRectangle.Width;
            }
            if (startPosition.X < endPosition.X)
            {
                startPosition.X -= headSquare.CollideRectangle.Width;
                endPosition.X += headSquare.CollideRectangle.Width;
            }
            var startCollider =
                new Square(new Rectangle(startPosition, new Size((int) headSquare.CollideRectangle.Width, 1)));
            var endCollider =
                new Square(new Rectangle(endPosition, new Size((int) headSquare.CollideRectangle.Width, 1)));
            foreach (var square in squares)
                square.OnStateUpdate += () =>
                {
                    square.Velocity = moveVector;
                    if (headSquare.IsIntersectsWith(startCollider) && !movingForward)
                    {
                        movingForward = true;
                        moveVector *= -1;
                    }
                    else if (headSquare.IsIntersectsWith(endCollider) && movingForward)
                    {
                        movingForward = false;
                        moveVector *= -1;
                    }
                };
        }
    }
}
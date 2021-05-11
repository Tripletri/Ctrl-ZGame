using System.Collections.Generic;
using System.Drawing;
using RobotVoyager.Engine;
using PointF = RobotVoyager.Engine.PointF;

namespace RobotVoyager.GameObjects
{
    internal class TimeTravelBox : Box, ITimeTraveler
    {
        private readonly Animator animator = new Animator();
        private readonly LimitedSizeQueue<PointF> pastPosition;
        private readonly AnimatedSprite shadow;

        public TimeTravelBox(Rectangle rectangle, int wrapDelay) : base(rectangle)
        {
            var path = "sprites/Objects/BoxShadow/Box";
            shadow = new AnimatedSprite(animator, rectangle, 10,
                new List<Image>
                {
                    new Bitmap(path + ".png"),
                    new Bitmap(path + "-2.png"),
                    new Bitmap(path + "-3.png"),
                    new Bitmap(path + "-2.png")
                },
                GetSprite().ZIndex - 1);
            pastPosition = new LimitedSizeQueue<PointF>(wrapDelay);
            OnStateUpdate += TimeTravelBoxOnStateUpdate;
        }

        public ICollider GoBackInTime()
        {
            Position = pastPosition.Last;
            pastPosition.Clear();
            return this;
        }

        public Sprite GetShadow()
        {
            return shadow;
        }

        private void TimeTravelBoxOnStateUpdate()
        {
            animator.UpdateAnimation();

            pastPosition.Add(Position);
            if (pastPosition.Last == Position)
            {
                shadow.Visibility = false;
                return;
            }
            shadow.Position = pastPosition.Last;
            shadow.Visibility = true;
        }
    }
}
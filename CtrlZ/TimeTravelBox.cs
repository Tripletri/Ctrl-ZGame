using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CtrlZ.Engine;
using PointF = CtrlZ.Engine.PointF;

namespace CtrlZ
{
    class TimeTravelBox : Box, ITimeTraveler
    {
        private AnimatedSprite shadow;
        private LimitedSizeQueue<PointF> pastPosition;
        private int wrapDelay;
        private Animator animator = new Animator();

        public TimeTravelBox(Rectangle rectangle, int wrapDelay) : base(rectangle)
        {
            this.wrapDelay = wrapDelay;
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

        public ICollider GoBackInTime()
        {
            Position= pastPosition.Last;
            pastPosition.Clear();
            return this;
        }

        public Sprite GetShadow()
        {
            return shadow;
        }
    }
}
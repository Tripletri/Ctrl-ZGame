using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ
{
    class AnimatedSprite : Sprite
    {
        private List<Image> images = new List<Image>();
        private int delay;
        private int ticks;
        private Image currentImage;
        private int currentImageId;

        public override Image Image => currentImage;

        public AnimatedSprite(Animator animator, Rectangle rectangle, int delay, List<Image> images) : base(rectangle)
        {
            this.delay = delay;
            currentImageId = 0;
            foreach (var image1 in images)
            {
                this.images.Add(ResizeImage(image1, rectangle.Width, rectangle.Height));
            }
            currentImage = this.images[currentImageId];
            animator.AnimationUpdated += Animator_AnimationUpdated;
        }

        private void Animator_AnimationUpdated()
        {
            ticks++;
            if (ticks < delay)
                return;
            ticks = 0;
            currentImageId++;
            currentImageId = currentImageId % images.Count;
            currentImage = images[currentImageId];
        }

        public AnimatedSprite(Animator animator, Rectangle rectangle, int delay, params Image[] images) : this(animator,
            rectangle, delay,
            images.ToList()) { }
    }
}
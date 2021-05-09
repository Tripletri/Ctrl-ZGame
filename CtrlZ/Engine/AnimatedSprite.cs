using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ.Engine
{
    internal class AnimatedSprite : Sprite
    {
        private Image currentImage;
        private int currentImageId;
        private readonly int delay;
        private readonly List<Image> images = new List<Image>();
        private int ticks;

        public AnimatedSprite(Animator animator, Rectangle rectangle, int delay, List<Image> images, int zIndex = 0) :
            base(rectangle)
        {
            this.delay = delay;
            currentImageId = 0;
            foreach (var image in images)
                this.images.Add(ResizeImage(image, rectangle.Width, rectangle.Height));
            currentImage = this.images[currentImageId];
            animator.AnimationUpdated += AnimatorAnimationUpdated;
            ZIndex = zIndex;
        }

        public AnimatedSprite(Animator animator, Rectangle rectangle, int delay, params Image[] images) : this(animator,
            rectangle, delay,
            images.ToList()) { }

        public override Image Image => currentImage;

        private void AnimatorAnimationUpdated()
        {
            ticks++;
            if (ticks < delay)
                return;
            ticks = 0;
            currentImageId++;
            currentImageId %= images.Count;
            currentImage = images[currentImageId];
        }
    }
}
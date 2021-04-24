using System;
using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ
{
    class Animator
    {
        public event Action AnimationUpdated;
        //public List<AnimatedSprite> Animations =  new List<AnimatedSprite>();

        //public Animator(Rectangle rectangle)
        //{

        //}

        public void UpdateAnimation()
        {
            AnimationUpdated?.Invoke();
        }
    }
}

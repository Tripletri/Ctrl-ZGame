using System;

namespace RobotVoyager.Engine
{
    internal class Animator
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
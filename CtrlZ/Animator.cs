using System;

namespace CtrlZ
{
    class Animator
    {
        public event Action AnimationUpdated;

        public void UpdateAnimation()
        {
            AnimationUpdated?.Invoke();
        }
    }
}

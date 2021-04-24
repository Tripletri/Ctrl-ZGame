using System;

namespace CtrlZ
{
    internal abstract class Transform
    {
        public PointF Position { get; set; }
        public event Action OnStateUpdate;
        protected Transform(PointF position)
        {
            Position = position;
        }

        public void UpdateState()
        {
            OnStateUpdate?.Invoke();
        }
    }
}
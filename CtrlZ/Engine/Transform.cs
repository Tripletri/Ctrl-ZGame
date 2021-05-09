using System;

namespace CtrlZ.Engine
{
    internal abstract class Transform
    {
        protected Transform(PointF position)
        {
            Position = position;
        }

        public PointF Position { get; set; }
        public event Action OnStateUpdate;

        public void UpdateState()
        {
            OnStateUpdate?.Invoke();
        }
    }
}
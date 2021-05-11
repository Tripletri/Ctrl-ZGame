using System;

namespace RobotVoyager.Engine
{
    internal abstract class Transform
    {
        public PointF Position { get; set; }

        protected Transform(PointF position)
        {
            Position = position;
        }

        public event Action OnStateUpdate;

        public void UpdateState()
        {
            OnStateUpdate?.Invoke();
        }
    }
}
using RobotVoyager.Engine;

namespace RobotVoyager.GameObjects
{
    internal interface ITimeTraveler
    {
        ICollider GoBackInTime();

        Sprite GetShadow();
    }
}
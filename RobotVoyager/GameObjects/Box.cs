using System.Drawing;
using RobotVoyager.Engine;

namespace RobotVoyager.GameObjects
{
    internal class Box : Square
    {
        public Box(Rectangle rectangle) : base(rectangle, new Bitmap("sprites/Objects/Box.png"))
        {
            Static = false;
            DecelerationSpeed = 0.3f;
            FrictionCoefficient = 0.95f;
        }
    }
}
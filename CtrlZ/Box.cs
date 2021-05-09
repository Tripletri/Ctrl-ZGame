using System.Drawing;
using CtrlZ.Engine;

namespace CtrlZ
{
    internal class Box : Square
    {
        public Box(Rectangle rectangle) : base(rectangle, new Bitmap("sprites/Objects/Box.png"))
        {
            Static = false;
        }
    }
}
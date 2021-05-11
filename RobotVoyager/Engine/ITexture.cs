using System.Drawing;

namespace RobotVoyager.Engine
{
    internal interface ITexture
    {
        int ZIndex { get; set; }
        Image Image { get; set; }
        bool Visibility { get; set; }
        bool ReversY { get; set; }
    }
}
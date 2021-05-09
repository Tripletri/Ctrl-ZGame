using System.Drawing;

namespace CtrlZ.Engine
{
    internal interface ITexture
    {
        int ZIndex { get; set; }
        Image Image { get; set; }
        bool Visibility { get; set; }
        bool ReversY { get; set; }
    }
}
using System.Drawing;

namespace CtrlZ
{
    interface ITexture
    {
        int ZIndex { get; set; }
        Image Image { get; set; }

        bool ReversY { get; set; }
    }
}
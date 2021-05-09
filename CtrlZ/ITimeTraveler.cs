using CtrlZ.Engine;

namespace CtrlZ
{
    internal interface ITimeTraveler
    {
        ICollider GoBackInTime();

        Sprite GetShadow();

    }
}
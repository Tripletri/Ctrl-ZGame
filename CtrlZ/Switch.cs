using System;
using System.Drawing;
using CtrlZ.Engine;

namespace CtrlZ
{
    internal class Switch : Square
    {
        private readonly Sprite offSprite;
        private readonly Action<Player> onPlayerEnter;
        private readonly Sprite onSprite;

        public Switch(Rectangle rectangle, Action<Player> onPlayerEnter) : base(rectangle)
        {
            onSprite = new Sprite(rectangle, new Bitmap("sprites/Objects/Switch (1).png"));
            offSprite = new Sprite(rectangle, new Bitmap("sprites/Objects/Switch (2).png"));
            this.onPlayerEnter = onPlayerEnter;
            Collision = false;
            Sprite.ZIndex = -1;
            Sprite = offSprite;
            OnTrigger += SwitchOnTrigger;
        }

        private void SwitchOnTrigger(ICollider collider)
        {
            if (!(collider is Player player))
                return;

            onPlayerEnter(player);
            Sprite = onSprite;
        }
    }
}
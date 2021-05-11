using System;
using System.Drawing;
using RobotVoyager.Engine;

namespace RobotVoyager.GameObjects
{
    internal class Switch : Square
    {
        private readonly Action<Player> onPlayerEnter;
        private readonly Sprite onSprite;

        public Switch(Rectangle rectangle, Action<Player> onPlayerEnter) : base(rectangle)
        {
            onSprite = new Sprite(rectangle, new Bitmap("sprites/Objects/Switch (1).png"));
            this.onPlayerEnter = onPlayerEnter;
            Collision = false;
            Sprite.ZIndex = -1;
            Sprite = new Sprite(rectangle, new Bitmap("sprites/Objects/Switch (2).png")); 
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
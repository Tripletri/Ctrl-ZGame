using System;
using System.Drawing;
using CtrlZ.Engine;

namespace CtrlZ
{
    internal class ExitDoor : Square
    {
        public Action OnLevelCompleted;
        public bool IsClose { get; private set; }
        private readonly Sprite closeSprite;
        private readonly Sprite openSprite;

        public ExitDoor(Rectangle rectangle) : base(rectangle)
        {
            openSprite = new Sprite(rectangle, new Bitmap("sprites/Objects/DoorOpen.png"));
            closeSprite = new Sprite(rectangle, new Bitmap("sprites/Objects/DoorLocked.png"));
            Sprite = openSprite;
            Collision = false;
            Sprite.ZIndex = -1;
            OnTrigger += ExitDoorOnTrigger;
        }

        public ExitDoor(Rectangle rectangle, bool isClose) : this(rectangle)
        {
            IsClose = isClose;
            if (isClose)
                Sprite = closeSprite;
        }

        public void Open()
        {
            IsClose = false;
            Sprite = openSprite;
        }

        public void Close()
        {
            IsClose = true;
            Sprite = closeSprite;
        }

        private void ExitDoorOnTrigger(ICollider otherCollider)
        { 
            if (otherCollider is Player && !IsClose)
                OnLevelCompleted?.Invoke();
        }
    }
}
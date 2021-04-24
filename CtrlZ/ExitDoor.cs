using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace CtrlZ
{
    class ExitDoor : Square
    {
        public Action OnLevelCompleted;
        public ExitDoor(Rectangle rectangle) : base(rectangle, new Bitmap("sprites/Objects/DoorOpen.png"))
        {
            Collision = false;
            Sprite.ZIndex= - 1;
            OnTrigger += ExitDoorOnTrigger;
        }

        private void ExitDoorOnTrigger(ICollider otherCollider)
        {
            if(otherCollider is Player)
                OnLevelCompleted?.Invoke();
        }
    }
}

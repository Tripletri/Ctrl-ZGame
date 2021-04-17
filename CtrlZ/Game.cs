using System.Collections.Generic;
using System.Drawing;

namespace CtrlZ
{
    class Game
    {
        private Rectangle clientSize;

        public Game(Rectangle clientSize)
        {
            this.clientSize = clientSize;
        }

        public Level CollisionTest()
        {
            var animator = new Animator();
            var background =
                new Sprite(new RectangleF(0, 0, clientSize.Width, clientSize.Height), new Bitmap("sprites/bg1.png"))
                {
                    ZIndex = -10
                };
            var platform2 = new Square(new RectangleF(0, 300, 500, 10));
            var wall = new Square(new RectangleF(10, 100, 10, 500));
            var wall1 = new Square(new Rectangle(200, 0, 100, 100),
                new AnimatedSprite(animator, new Rectangle(0, 0, 100, 100), 50,
                    new List<Image>
                    {
                        new Bitmap("sprites/default.jpg"),
                        new Bitmap("sprites/default2.jpg"),
                        new Bitmap("sprites/default3.jpg"),
                        new Bitmap("sprites/default4.jpg"),

                    })) { Static = false, };
            var wall2 = new Square(new RectangleF(250, 270, 20, 20)) { Static = false, };
            var player = new Player(new Rectangle(30, 200, 20, 40)) { Static = false, FrictionCoefficient = 0 };

            return new Level(new List<Transform>
                {
                    background,
                    platform2,
                    wall,
                    wall1,
                    wall2
                }, player, animator
            );
        }
    }
}
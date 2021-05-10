using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CtrlZ.Engine;

namespace CtrlZ
{
    internal class Level
    {
        private readonly List<ICollider> colliders = new List<ICollider>();
        private readonly List<Transform> gameObjects = new List<Transform>();
        private Animator animator;
        public Action OnLevelComplete;

        public string Name { get; set; }
        public int GameTicks { get; private set; }

        public Player Player { get; private set; }

        public IEnumerable<Transform> GetObjects => gameObjects;

        public Level(Size clientSize, string name = "New level")
        {
            Name = name;
            var floorPrefab = new List<Square>();
            var playerAnimator = new Animator();
            var backgroundPrefab = new Sprite(new RectangleF(0, 0, clientSize.Width, clientSize.Height),
                new Bitmap("sprites/background.png")) { ZIndex = -10 };
            const float scale = 0.25f;
            var playerPrefab =
                new Player(new Rectangle(30, 735, (int) (230 * scale), (int) (500 * scale)), playerAnimator)
                {
                    ColliderArea = new RectangleF(52 * scale, 105 * scale, 128 * scale, 394 * scale)
                };

            for (var i = 0; i < clientSize.Width; i += 128)
                floorPrefab.Add(new Square(new Rectangle(i, 896 - 64, 128, 128),
                    new Bitmap("sprites/Tiles/Tile (2).png")));

            var objects = new List<Transform>
            {
                backgroundPrefab,
                new Square(new Rectangle(0, 0, 2, clientSize.Height)),
                new Square(new Rectangle(clientSize.Width - 2, 0, 2, clientSize.Height))
            };
            objects.AddRange(floorPrefab);
            InitializeLevel(objects, playerPrefab, playerAnimator);
        }

        public Level(IList<Transform> objects, Player player, Animator animator)
        {
            InitializeLevel(objects, player, animator);
        }

        private void InitializeLevel(IList<Transform> objects, Player player, Animator initAnimator)
        {
            AddObject(player);
            AddObjects(objects);
            //AddObject(exit);
            Player = player;
            animator = initAnimator;
        }

        public void AddObject(Transform transform)
        {
            gameObjects.Add(transform);
            if (transform is ICollider collider)
                colliders.Add(collider);
            if (transform is ITimeTraveler timeTraveler)
                gameObjects.Add(timeTraveler.GetShadow());
            if (transform is ExitDoor exitDoor)
                exitDoor.OnLevelCompleted += () => OnLevelComplete?.Invoke();
        }

        public void AddObjects(IEnumerable<Transform> transforms)
        {
            foreach (var transform in transforms)
                AddObject(transform);
        }

        public void Update()
        {
            GameTicks++;
            animator.UpdateAnimation();
            foreach (var gameObject in gameObjects)
            {
                if (gameObject is ICollider collider)
                {
                    collider.MovePhysics(colliders);
                    collider.Move(colliders);
                }
                gameObject.UpdateState();
            }
        }

        public void RewindTime()
        {
            foreach (var timeTraveler in gameObjects.OfType<ITimeTraveler>())
            {
                var pastCollider = timeTraveler.GoBackInTime();
                if (pastCollider.CollideRectangle.IntersectsWith(Player.CollideRectangle))
                    Player.Die("Death by time travel paradox");
                if (colliders.Any(collider => timeTraveler != collider
                                              && !(collider is Player)
                                              && pastCollider.CollideRectangle
                                                  .IntersectsWith(
                                                      collider.CollideRectangle)))
                    Player.Die(
                        "A time-traveling object collided with another object, resulting in a distortion of space-time.");
            }
        }
    }
}
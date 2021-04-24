using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CtrlZ
{
    class Level
    {
        public string Name { get; set; }
        public int GameTicks { get; private set; }

        private readonly List<Transform> gameObjects = new List<Transform>();
        private List<ICollider> colliders = new List<ICollider>();
        private Animator animator;


        public Player Player { get; private set; }
        public Action OnLevelComplete;

        public Level(Size clientSize, PointF exitPosition)
        {
            var floorPrefab = new List<Square>();
            var playerAnimator = new Animator();
            var backgroundPrefab = new Sprite(new RectangleF(0, 0, clientSize.Width, clientSize.Height),
                new Bitmap("sprites/background.png")) { ZIndex = -10 };
            var playerPrefab = new Player(new Rectangle(30, 600, 64, 164), playerAnimator)
            {
                Static = false, ColliderArea = new RectangleF(14,74,36,92)
            };
            
            for (var i = 0; i < clientSize.Width; i += 128)
            {
                floorPrefab.Add(new Square(new Rectangle(i, 896 - 64, 128, 128),
                    new Bitmap("sprites/Tiles/Tile (2).png")));
            }
            var exitPrefab = new ExitDoor(new Rectangle((int) exitPosition.X, (int) exitPosition.Y, 85, 140));

            var objects = new List<Transform>
            {
                backgroundPrefab,
                new Square(new Rectangle(0, 0, 2, clientSize.Height)),
                new Square(new Rectangle(clientSize.Width - 2, 0, 2, clientSize.Height)),
            };
            objects.AddRange(floorPrefab);
            exitPrefab.OnLevelCompleted += () => OnLevelComplete?.Invoke();
            InitializeLevel(objects, playerPrefab, exitPrefab, playerAnimator);
        }

        public Level(IList<Transform> objects, Player player, ExitDoor exit, Animator animator)
        {
            InitializeLevel(objects, player, exit, animator);
        }

        private void InitializeLevel(IList<Transform> objects, Player player, ExitDoor exit, Animator initAnimator)
        {
            AddObject(player);
            AddObjects(objects);
            AddObject(exit);
            Player = player;
            animator = initAnimator;
        }

        public void AddObject(Transform transform)
        {
            gameObjects.Add(transform);
            if (transform is ICollider collider)
                colliders.Add(collider);
        }

        public void AddObjects(IEnumerable<Transform> transforms)
        {
            foreach (var transform in transforms)
                AddObject(transform);
        }

        public IEnumerable<Transform> GetObjects => gameObjects;

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

        public void WarpTime()
        {
            foreach (var timeTraveler in gameObjects.OfType<ITimeTraveler>())
            {
                timeTraveler.GoBackInTime(10);
            }
        }
    }
}
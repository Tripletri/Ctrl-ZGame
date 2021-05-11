using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using RobotVoyager.Engine;
using RobotVoyager.GameObjects;
using PointF = RobotVoyager.Engine.PointF;

namespace RobotVoyager
{
    internal class Game
    {
        private readonly InputManager[] inputManagers;
        public readonly LinkedList<Func<Level>> LevelOrder = new LinkedList<Func<Level>>();
        private readonly Sprite settings;

        private Rectangle clientSize;
        private Func<Level> currentLevel;
        private Action onCurrentLevelComplete;
        public Level CurrentLevel { get; private set; }

        public Game(Rectangle clientSize, params InputManager[] inputManagers)
        {
            this.clientSize = clientSize;
            this.inputManagers = inputManagers;

            settings = new Sprite(clientSize) { Visibility = false };
        }

        public event Action OnLevelChanged;

        public void LoadNextLevel()
        {
            var currentLevelNode = LevelOrder.Find(currentLevel);
            if (currentLevelNode?.Next == null)
                return;
            var nextLevelLoader = currentLevelNode.Next.Value;
            LoadLevel(nextLevelLoader, LoadNextLevel);
        }

        public void AddLevels(params Func<Level>[] levels)
        {
            foreach (var level in levels)
                LevelOrder.AddLast(level);
        }

        private string GetInputDescription()
        {
            var stringBuilder = new StringBuilder();
            foreach (var inputManager in inputManagers)
            foreach (var bind in inputManager.Binds.GroupBy(bind => bind.Description))
            {
                var binds = bind.ToArray();
                for (var i = 0; i < binds.Length; i++)
                {
                    stringBuilder.Append(binds[i].Key);
                    if (i != binds.Length - 1)
                        stringBuilder.Append(",");
                }
                stringBuilder.Append($"-{bind.Key}\n");
            }
            return stringBuilder.ToString();
        }

        public void ShowSetting()
        {
            if (settings.Visibility)
            {
                settings.Visibility = false;
            }
            else
            {
                var textBitmap = new Bitmap(clientSize.Width, clientSize.Height);
                DrawText(textBitmap, GetInputDescription(), clientSize);
                settings.Image = textBitmap;
                settings.Visibility = true;
            }
        }

        public void LoadLevel(Func<Level> levelLoader, Action onLevelComplete = null)
        {
            currentLevel = levelLoader;
            onCurrentLevelComplete = onLevelComplete;
            var level = levelLoader();
            if (onLevelComplete != null)
                level.OnLevelComplete = onLevelComplete;
            level.AddObject(settings);
            CurrentLevel = level;
            OnLevelChanged?.Invoke();
        }

        public void RestartCurrentLevel()
        {
            LoadLevel(currentLevel, onCurrentLevelComplete);
        }

        public Level IntroLevel()
        {
            var lvl = new Level(clientSize.Size, "Salutations, voyageur");
            var exitDoor = new ExitDoor(new Rectangle(1389, 693, 85, 140));
            lvl.AddObject(exitDoor);
            var textBitmap = new Bitmap(clientSize.Width, clientSize.Height);
            DrawText(textBitmap,
                "Use Escape to see controls.\nThe goal is to get to the door.\nAnd beware of time paradoxes!",
                clientSize, StringAlignment.Near);
            lvl.AddObject(new Sprite(
                new RectangleF(clientSize.X, clientSize.Y + 100, clientSize.Width, clientSize.Height), textBitmap));
            return lvl;
        }

        public static void DrawText(Bitmap textBitmap, string text, Rectangle size,
            StringAlignment alignment = StringAlignment.Center)
        {
            var graphics = Graphics.FromImage(textBitmap);
            DrawText(graphics, text, size, alignment);
        }

        public static void DrawText(Graphics graphics, string text, Rectangle size,
            StringAlignment lineAlignment = StringAlignment.Center)
        {
            var font = new Font(FontFamily.GenericMonospace, 50, FontStyle.Regular, GraphicsUnit.Pixel);
            var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = lineAlignment };
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(text, font, Brushes.White, size, stringFormat);
        }

        #region Levels

        public Level Level1()
        {
            var lvl = new Level(clientSize.Size, "The first is the first");
            lvl.AddObject(new ExitDoor(new Rectangle(1238, 435, 85, 140)));
            lvl.AddObject(new TimeTravelBox(new Rectangle(1370, 717, 115, 115), 200));
            lvl.AddObjects(new Platform(new PointF(848, 640), 2).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1102, 575), 2).CreatePlatform());
            return lvl;
        }

        public Level Level2()
        {
            var lvl = new Level(clientSize.Size, "Big box");
            lvl.AddObject(new ExitDoor(new Rectangle(221, 300, 85, 140)));
            lvl.AddObjects(new Platform(new PointF(189, 440), 2).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(503, 717), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(822, 566), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(950, 501), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(758, 394), 1).CreatePlatform());
            lvl.AddObject(new TimeTravelBox(new Rectangle(477, 541, 180, 180), 110));
            return lvl;
        }

        public Level Level3()
        {
            var lvl = new Level(clientSize.Size, "Platforms");
            var exitDoor = new ExitDoor(new Rectangle(25, 113, 85, 140), true);
            lvl.AddObject(new Switch(new Rectangle(1365, 661, 48, 171), player => exitDoor.Open()));
            lvl.AddObjects(new Platform(new PointF(0, 566), 3).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(0, 253), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(506, 566), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(694, 693), 3).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1073, 628), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(884, 440), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(566, 253), 1).CreatePlatform());
            lvl.AddObjects(new MovingPlatform(new PointF(694, 375), new PointF(694, 253), 1.5f, 1).CreatePlatform());
            lvl.AddObjects(new MovingPlatform(new PointF(441, 253), new PointF(131, 253), 3, 1).CreatePlatform());
            lvl.AddObject(new TimeTravelBox(new Rectangle(1079, 517, 115, 115), 240));
            lvl.Player.Position = new PointF(131, 469);
            lvl.AddObject(exitDoor);
            return lvl;
        }

        public Level Level4()
        {
            var lvl = new Level(clientSize.Size, "Level 4");
            var exitDoor = new ExitDoor(new Rectangle(148, 174, 85, 140), true);
            lvl.AddObject(new Switch(new Rectangle(291, 661, 48, 171), player => exitDoor.Open()));
            lvl.AddObject(exitDoor);
            lvl.AddObjects(new MovingPlatform(new PointF(1010, 566), new PointF(1010, 253), 3, 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(126, 314), 3).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(567, 505), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(758, 631), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(822, 253), 1).CreatePlatform());
            lvl.Player.Position = new PointF(403, 217);
            lvl.AddObject(new TimeTravelBox(new Rectangle(574, 391, 115, 115), 300));
            return lvl;
        }

        public Level Level5()
        {
            var lvl = new Level(clientSize.Size, "Carefully");
            var exitDoor = new ExitDoor(new Rectangle(84, 333, 85, 140), true);
            lvl.AddObject(new Switch(new Rectangle(1445, 660, 48, 171), player => exitDoor.Open()));
            lvl.AddObject(exitDoor);
            lvl.AddObjects(new Platform(new PointF(251, 725), 1).CreatePlatform());
            lvl.AddObjects(new MovingPlatform(new PointF(441, 660), new PointF(441, 346), 3, 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(630, 620), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1097, 379), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(697, 346), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1121, 652), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1249, 587), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(825, 281), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(63, 473), 2).CreatePlatform());
            lvl.AddObject(new TimeTravelBox(new Rectangle(1104, 264, 115, 115), 300));
            lvl.AddObject(new Box(new Rectangle(952, 662, 169, 169)));
            return lvl;
        }

        public Level Level6()
        {
            var lvl = new Level(clientSize.Size, "Boxes");
            var exitDoor = new ExitDoor(new Rectangle(1412, 553, 85, 140), true);
            lvl.AddObject(new Switch(new Rectangle(291, 143, 48, 171), player => exitDoor.Open()));
            lvl.AddObject(exitDoor);
            lvl.AddObjects(new Platform(new PointF(251, 314), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(563, 566), 2).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(883, 613), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(946, 407), 1).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1390, 693), 1).CreatePlatform());
            lvl.AddObject(new TimeTravelBox(new Rectangle(563, 439, 127, 127), 200));
            lvl.AddObject(new TimeTravelBox(new Rectangle(698, 451, 115, 115), 200));
            lvl.AddObject(new TimeTravelBox(new Rectangle(954, 292, 115, 115), 200));
            lvl.Player.Position = new PointF(603, 342);
            return lvl;
        }

        public Level LastLevel()
        {
            var lvl = new Level(clientSize.Size, "See you, voyageur!");
            var textBitmap = new Bitmap(clientSize.Width, clientSize.Height);
            DrawText(textBitmap,
                "This is the end of your journey.\nSee you, voyageur!",
                clientSize, StringAlignment.Near);
            lvl.AddObject(new Sprite(
                new RectangleF(clientSize.X, clientSize.Y + 100, clientSize.Width, clientSize.Height), textBitmap));
            return lvl;
        }

        #endregion
    }
}
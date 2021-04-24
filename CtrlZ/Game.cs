using System;
using System.Drawing;

namespace CtrlZ
{
    class Game
    {
        private Rectangle clientSize;
        public Level CurrentLevel { get; private set; }
        private Func<Level> currentLevel;
        private Action onCurrentLevelComplete = null;

        public Game(Rectangle clientSize)
        {
            this.clientSize = clientSize;
        }

        public void LoadLevel(Func<Level> levelLoader, Action onLevelComplete = null)
        {
            currentLevel = levelLoader;
            onCurrentLevelComplete = onLevelComplete;
            var level = levelLoader();
            if (onLevelComplete != null)
                level.OnLevelComplete = onLevelComplete;
            CurrentLevel = level;
        }

        public void RestartCurrentLevel()
        {
            LoadLevel(currentLevel, onCurrentLevelComplete);
        }

        public Level IntroLevel()
        {
            return new Level(clientSize.Size, new PointF(1389, 693));
        }

        public Level Level1()
        {
            var lvl = new Level(clientSize.Size, new PointF(1169, 435));
            lvl.AddObject(new Box(new Rectangle(1313, 717, 115, 115)));
            lvl.AddObjects(new Platform(new PointF(896, 640), 2).CreatePlatform());
            lvl.AddObjects(new Platform(new PointF(1152, 575), 1).CreatePlatform());
            return lvl;
        }
    }
}
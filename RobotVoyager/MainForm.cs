using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using RobotVoyager.Engine;
using RobotVoyager.GameObjects;
using PointF = System.Drawing.PointF;

namespace RobotVoyager
{
    public partial class MainForm : Form
    {
        private const int delay = 10;
        private readonly Game game;
        private readonly Stopwatch mainStopwatch = new Stopwatch();
        private readonly InputManager playerInputManager = new InputManager();
        private readonly InputManager systemInputManager = new InputManager();

        private bool deathHandled;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            ClientSize = new Size(1536, 896);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            mainStopwatch.Start();

            game = new Game(ClientRectangle, playerInputManager, systemInputManager);
            game.OnLevelChanged += () => Text = game.CurrentLevel.Name;

            #region Levels

            Func<Level> introLevel = game.IntroLevel;
            game.AddLevels(introLevel, game.Level1, game.Level2, game.Level3, game.Level4, game.Level5, game.Level6,
                game.LastLevel);
            game.LoadLevel(introLevel, game.LoadNextLevel);

            #endregion

            #region Frames

            var timer = new Timer { Interval = 1 };
            Paint += ProcessFrame;
            timer.Tick += (s, e) => Invalidate();
            timer.Start();

            #endregion

            #region Controls

            playerInputManager.AddBind(Key.A, () => game.CurrentLevel.Player.Move(-1), description: "Move");
            playerInputManager.AddBind(Key.D, () => game.CurrentLevel.Player.Move(1), description: "Move");
            playerInputManager.AddBind(Key.W, () => game.CurrentLevel.Player.Jump(), false, "Jump");
            playerInputManager.AddBind(Key.Space, () => game.CurrentLevel.Player.Jump(), false, "Jump");
            playerInputManager.AddBind(Key.Z, () => game.CurrentLevel.RewindTime(), false, "Rewind Time");
            playerInputManager.AddBind(Key.E, () => game.CurrentLevel.RewindTime(), false, "Rewind Time");
            playerInputManager.AddBind(Key.Q, () => game.CurrentLevel.RewindTime(), false, "Rewind Time");
            systemInputManager.AddBind(Key.R, () => game.RestartCurrentLevel(), false, "Restart Level");
            systemInputManager.AddBind(Key.Escape, () => game.ShowSetting(), false, "See Controls");

            #if DEBUG
            systemInputManager.AddBind(Key.N, () => game.LoadNextLevel());
            #endif

            #endregion
        }

        private void ProcessFrame(object sender, PaintEventArgs e)
        {
            if (mainStopwatch.ElapsedMilliseconds > delay)
            {
                if (game.CurrentLevel.Player.Alive)
                    playerInputManager.ProceedControl();
                systemInputManager.ProceedControl();
                game.CurrentLevel.Update();
                mainStopwatch.Restart();
            }
            DrawFrame(e.Graphics);
        }

        private void DrawFrame(Graphics graphics)
        {
            foreach (var (transform, texture) in game.CurrentLevel.GetObjects.Where(gameObject => gameObject is ISprite)
                .Select(gameObject => Tuple.Create(gameObject,
                    (ITexture) ((ISprite) gameObject).GetSprite()))
                .OrderBy(tuple => tuple.Item2.ZIndex))
            {
                if (texture.Visibility)
                    DrawTexture(graphics, texture, transform);
                if (!game.CurrentLevel.Player.Alive && !deathHandled)
                {
                    Game.DrawText(graphics, game.CurrentLevel.Player.DeathReason + "\n Use R to restart",
                        ClientRectangle);
                    deathHandled = true;
                }
                else
                {
                    deathHandled = false;
                }
            }
            #if DEBUG
            graphics.DrawString(game.CurrentLevel.GameTicks.ToString(), new Font(FontFamily.GenericMonospace, 16),
                Brushes.GreenYellow, 0, 0);
            Debug.WriteLine(game.CurrentLevel.Player.CollideRectangle.Bottom);
            #endif
        }

        private static void DrawTexture(Graphics graphics, ITexture texture, Transform transform)
        {
            if (!texture.ReversY)
                graphics.DrawImage(texture.Image,
                    new PointF[]
                    {
                        transform.Position,
                        new Engine.PointF(transform.Position.X + texture.Image.Width,
                            transform.Position.Y),
                        new Engine.PointF(transform.Position.X,
                            transform.Position.Y + texture.Image.Height)
                    });
            else
                graphics.DrawImage(texture.Image,
                    new PointF[]
                    {
                        new Engine.PointF(
                            transform.Position.X + texture.Image.Width,
                            transform.Position.Y),
                        transform.Position,
                        new Engine.PointF(
                            transform.Position.X + texture.Image.Width,
                            transform.Position.Y + texture.Image.Height)
                    });
            #if DEBUG
            if (transform is ICollider collider)
                graphics.DrawRectangle(Pens.Red,
                    new Rectangle((int) collider.CollideRectangle.X,
                        (int) collider.CollideRectangle.Y,
                        (int) collider.CollideRectangle.Width,
                        (int) collider.CollideRectangle.Height));
            #endif
        }
    }
}
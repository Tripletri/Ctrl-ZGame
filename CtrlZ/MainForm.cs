using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace CtrlZ
{
    public partial class MainForm : Form
    {
        private Game game;
        private Stopwatch stopwatch = new Stopwatch();
        private const int delay = 10;
        private bool wWasReleased = true;

        public MainForm()
        {
            InitializeComponent();

            stopwatch.Start();

            ClientSize = new Size(1536,
                896);
            StartPosition = FormStartPosition.CenterScreen;
            game = new Game(ClientRectangle);
            game.LoadLevel(game.IntroLevel, () => game.LoadLevel(game.Level1));
            game.LoadLevel(game.Level1);
            DoubleBuffered = true;
            var timer = new Timer { Interval = 1 };
            Paint += ProcessFrame;
            timer.Tick += (s, e) => Invalidate();
            timer.Start();
        }

        private void ProcessFrame(object sender, PaintEventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > delay)
            {
                ProceedControl();
                game.CurrentLevel.Update();
                stopwatch.Restart();
            }

            var graphics = e.Graphics;
            DrawFrame(graphics);
        }

        private void DrawFrame(Graphics graphics)
        {
            foreach (var (transform, texture) in game.CurrentLevel.GetObjects.Where(gameObject => gameObject is ISprite)
                .Select(gameObject => (Tuple.Create(gameObject,
                    (ITexture) ((ISprite) gameObject).GetSprite())))
                .OrderBy(tuple => tuple.Item2.ZIndex))
            {
                if (!texture.ReversY)
                {
                    graphics.DrawImage(texture.Image,
                        new System.Drawing.PointF[]
                        {
                            transform.Position,
                            new PointF(transform.Position.X + texture.Image.Width,
                                transform.Position.Y),
                            new PointF(transform.Position.X,
                                transform.Position.Y + texture.Image.Height)
                        });
                }
                else
                {
                    graphics.DrawImage(texture.Image,
                        new System.Drawing.PointF[]
                        {
                            new PointF(
                                transform.Position.X + texture.Image.Width,
                                transform.Position.Y),
                            transform.Position,
                            new PointF(
                                transform.Position.X + texture.Image.Width,
                                transform.Position.Y + texture.Image.Height)
                        });
                }

                #if DEBUG
                if (transform is ICollider collider)

                    graphics.DrawRectangle(Pens.Red,
                        new Rectangle((int) collider.CollideRectangle.X,
                            (int) collider.CollideRectangle.Y,
                            (int) collider.CollideRectangle.Width,
                            (int) collider.CollideRectangle.Height));
                #endif
            }
            #if DEBUG
            graphics.DrawString(game.CurrentLevel.GameTicks.ToString(), new Font(FontFamily.GenericMonospace, 16),
                Brushes.GreenYellow, 0, 0);
            #endif
        }

        private void ProceedControl()
        {
            var x = Keyboard.IsKeyDown(Key.D) ? 1 : 0;
            x += Keyboard.IsKeyDown(Key.A) ? -1 : 0;
            game.CurrentLevel.Player.Move(x);
            if (Keyboard.IsKeyDown(Key.W) && wWasReleased)
            {
                game.CurrentLevel.Player.Jump();
                wWasReleased = false;
            }
            else if (Keyboard.IsKeyUp(Key.W))
                wWasReleased = true;
            if (Keyboard.IsKeyDown(Key.R))
                game.RestartCurrentLevel();
            if(Keyboard.IsKeyDown(Key.Z))
                game.CurrentLevel.WarpTime();
        }
    }
}
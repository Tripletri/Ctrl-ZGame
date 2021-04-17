using System;
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
        private Level currentLevel;
        private Stopwatch stopwatch = new Stopwatch();
        private int delay = 10;

        public MainForm()
        {
            InitializeComponent();

            stopwatch.Start();

            ClientSize = new Size(800, 600);
            game = new Game(ClientRectangle);
            currentLevel = game.CollisionTest();
            DoubleBuffered = true;
            var timer = new Timer { Interval = 1 };
            Paint += DrawFrame;
            timer.Tick += (s, e) => Invalidate();
            timer.Start();
        }

        private void DrawFrame(object sender, PaintEventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > delay)
            {
                var x = Keyboard.IsKeyDown(Key.D) ? 1 : 0;
                x += Keyboard.IsKeyDown(Key.A) ? -1 : 0;
                currentLevel.Player.Move(x);
                if (Keyboard.IsKeyDown(Key.W))
                    currentLevel.Player.Jump();
                currentLevel.Update();
                stopwatch.Restart();
            }
            var graphics = e.Graphics;
            //foreach (var sprite in currentLevel.Objects.Cast<ISprite>())
            //    .OrderBy(obj => obj))
            //{
            //    graphics.DrawImage(sprite.Image, ((Transform) sprite).Position);
            //}
            foreach (var gameObject in currentLevel.Objects.Where(gameObject => gameObject is ISprite)
                .Select(gameObject => (Tuple.Create(gameObject, (ITexture)((ISprite) gameObject).GetSprite())))
                .OrderBy(tuple => tuple.Item2.ZIndex))
            {
                graphics.DrawImage(gameObject.Item2.Image, gameObject.Item1.Position);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentLevel = game.CollisionTest();
        }
    }
}
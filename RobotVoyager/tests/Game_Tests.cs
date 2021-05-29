using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using RobotVoyager.Engine;
using RobotVoyager.GameObjects;
using PointF = RobotVoyager.Engine.PointF;

namespace RobotVoyager.tests
{
    [TestFixture]
    class Game_Tests
    {
        private Game game;
        private PointF playerInitialPosition;
        private Player player;
        private int floorLevel;
        private Square testSquare;
        private Rectangle clientSize = new Rectangle(0, 0, 800, 600);
        private const float delta = 0.001f;

        static object[] moveCases = { 1, -1 };

        static object[] platformMoveCases =
        {
            new object[] { 100, 300, 300, 300 },
            new object[] { 300, 300, 100, 300 },
            new object[] { 100, 100, 100, 300 },
            new object[] { 300, 100, 100, 100 },
            new object[] { 100, 300, 300, 100 },
            new object[] { 300, 100, 100, 300 },
            new object[] { 100, 100, 300, 300 },
            new object[] { 300, 300, 100, 100 },
        };

        [SetUp]
        public void SetUp()
        {
            game = new Game(clientSize, null);

            const float playerSizeScale = 0.25f;
            var playerPrefab =
                new Player(new Rectangle(30, 735, (int) (230 * playerSizeScale), (int) (500 * playerSizeScale)),
                    new Animator())
                {
                    ColliderArea = new RectangleF(52 * playerSizeScale, 105 * playerSizeScale, 128 * playerSizeScale,
                        394 * playerSizeScale)
                };
            var floorPrefab = new List<Transform>();
            floorLevel = (int) playerPrefab.CollideRectangle.Bottom;
            for (var i = 0; i < clientSize.Width; i += 128)
                floorPrefab.Add(new Square(new Rectangle(i, floorLevel, 128, 128),
                    new Bitmap(1, 1)));
            var square = new Square(new Rectangle(100, floorLevel - 1, 100, 100)) { Static = false };
            testSquare = square;
            game.LoadLevel(() =>
            {
                var lvl = new Level(floorPrefab, playerPrefab, new Animator());
                lvl.AddObject(square);
                return lvl;
            });
            UpdateLevel(game.CurrentLevel, 20);
            playerInitialPosition = game.CurrentLevel.Player.Position;
            player = game.CurrentLevel.Player;
            player.Collision = false;
            player.Static = true;
        }

        private void ActivatePlayer()
        {
            player.Collision = true;
            player.Static = false;
        }

        private void MovePlayer(int x, int times)
        {
            for (var i = 0; i < times; i++)
            {
                player.Move(x);
                game.CurrentLevel.Update();
            }
        }

        [TestCaseSource(nameof(moveCases))]
        public void Player_CanMoveHorizontally(int x)
        {
            ActivatePlayer();
            MovePlayer(x, 10);
            var playerMoveDelta = game.CurrentLevel.Player.Position - playerInitialPosition;
            if (GetSing(playerMoveDelta.X) == GetSing(x))
                Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public void Player_CanJump()
        {
            ActivatePlayer();
            player.Jump();
            game.CurrentLevel.Update();
            Assert.Greater(playerInitialPosition.Y, player.Position.Y);
        }

        [Test]
        public void Square_DontFallThroughWalls()
        {
            testSquare.Position.Y = 0;
            UpdateLevel(game.CurrentLevel, 100);
            Assert.AreEqual(floorLevel, testSquare.CollideRectangle.Bottom);
        }

        [Test]
        public void Square_CanFall()
        {
            const int startY = 0;
            testSquare.Position.Y = startY;
            UpdateLevel(game.CurrentLevel, 100);
            Assert.Less(startY, testSquare.Position.Y);
        }

        [Test]
        public void Player_CantJumpThroughWalls()
        {
            ActivatePlayer();
            var ceiling = new Square(new Rectangle((int) playerInitialPosition.X, (int) player.CollideRectangle.Top,
                100,
                20));
            game.CurrentLevel.AddObject(ceiling);
            var isReachCeiling = false;
            for (var i = 0; i < 100; i++)
            {
                player.Jump();
                game.CurrentLevel.Update();
                Assert.GreaterOrEqual(player.CollideRectangle.Top, ceiling.CollideRectangle.Bottom);
                if (Math.Abs(player.CollideRectangle.Top - ceiling.CollideRectangle.Bottom) < delta)
                    isReachCeiling = true;
            }
            Assert.True(isReachCeiling, nameof(isReachCeiling));
        }

        [Test]
        public void Player_CantJumpTwice()
        {
            ActivatePlayer();
            player.Jump();
            game.CurrentLevel.Update();
            while (player.Velocity.Y < 0)
                game.CurrentLevel.Update();
            player.Jump();
            game.CurrentLevel.Update();
            Assert.Greater(player.Velocity.Y, 0);
        }

        [TestCaseSource(nameof(moveCases))]
        public void Square_HitsWallsHorizontally(int x)
        {
            var wallPosition = testSquare.Position + new PointF(20 * GetSing(x), 0);
            if (x > 0)
                wallPosition.X += testSquare.CollideRectangle.Width;
            var wall = new Square(new Rectangle(wallPosition, new Size(50, 300)));
            game.CurrentLevel.AddObject(wall);
            game.CurrentLevel.AddObject(testSquare);

            testSquare.Velocity.X = x;

            for (var i = 0; i < 100; i++)
            {
                game.CurrentLevel.Update();
                CheckWallGlitching(x, wall, testSquare);
            }
            IsSquareStopsByWall(x, wall, testSquare);
        }

        private void IsSquareStopsByWall(int x, Square wall, Square square)
        {
            if (x > 0)
                Assert.AreEqual(wall.CollideRectangle.Left, square.CollideRectangle.Right, delta);
            if (x < 0)
                Assert.AreEqual(wall.CollideRectangle.Right, square.CollideRectangle.Left, delta);
        }

        private void CheckWallGlitching(int x, Square wall, Square square)
        {
            const string error = "Glitching through wall.";

            if (x < 0)
                Assert.GreaterOrEqual(square.CollideRectangle.Right, wall.CollideRectangle.Left, error);
            if (x > 0)
                Assert.LessOrEqual(square.CollideRectangle.Left, wall.CollideRectangle.Right, error);
        }

        [TestCaseSource(nameof(moveCases))]
        public void Square_StopsWithTime(int x)
        {
            testSquare.Velocity.X = x;
            testSquare.DecelerationSpeed = 0.2f;
            UpdateLevel(game.CurrentLevel, 100);
            Assert.AreEqual(new PointF(0, 0), testSquare.Velocity);
        }

        [TestCaseSource(nameof(moveCases))]
        public void Square_MoveSquareOnTop(int x)
        {
            var topSquare = new Square(new Rectangle((int) testSquare.Position.X,
                (int) (testSquare.Position.Y - testSquare.CollideRectangle.Height),
                (int) testSquare.CollideRectangle.Width,
                (int) testSquare.CollideRectangle.Height)) { Static = false };
            game.CurrentLevel.AddObject(topSquare);
            testSquare.Velocity.X = x;
            UpdateLevel(game.CurrentLevel, 50);
            Assert.AreEqual(testSquare.Position.X, topSquare.Position.X, delta);
        }

        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, 2)]
        [TestCase(-1, 2)]
        [TestCase(-1, 3)]
        [TestCase(1, 3)]
        public void Square_PushOtherSquare(int x, int count)
        {
            var lvl = new Level(clientSize.Size, "Test level");
            var squares = CreateSquares(x, count, x > 0 ? 0 : clientSize.Width);
            var squareToCheck = squares.Last();
            var squareToPush = squares.First();
            var wallX = x > 0 ? squareToCheck.CollideRectangle.Right + 50 : squareToCheck.CollideRectangle.Left - 100;
            var wall = new Square(new Rectangle((int) wallX, floorLevel - 200, 50, 300));
            lvl.AddObjects(squares);
            lvl.AddObject(wall);
            PushSquareAndCheck(x, lvl, wall, squareToCheck, squareToPush);
        }

        private void PushSquareAndCheck(int x, Level lvl, Square wall, Square squareToCheck, Square squareToPush)
        {
            game.LoadLevel(() => lvl);
            for (int i = 0; i < 100; i++)
            {
                game.CurrentLevel.Update();
                CheckWallGlitching(x, wall, squareToCheck);
                squareToPush.Velocity.X = x * 10;
            }
            IsSquareStopsByWall(x, wall, squareToCheck);
        }

        private List<Square> CreateSquares(int x, int count, int startPositionX)
        {
            var squares = new List<Square>();
            for (var i = 1; i <= count; i++)
            {
                var square = new Square(new Rectangle(startPositionX + (100 * i + 10) * GetSing(x), floorLevel - 101,
                    100,
                    100)) { Static = false, DecelerationSpeed = 0.2f };
                squares.Add(square);
            }
            return squares;
        }

        [TestCaseSource(nameof(platformMoveCases))]
        public void Platform_MovePlayer(float startX, float startY, float endX, float endY)
        {
            var lvl = new Level(clientSize.Size);
            var startPosition = new PointF(startX, startY);
            var endPosition = new PointF(endX, endY);
            var movingPlatform = AddPlayerOnMovingPlatform(lvl, startPosition, endPosition);
            game.LoadLevel(() => lvl);
            for (var i = 0; i < 500; i++)
            {
                game.CurrentLevel.Update();
                Assert.AreEqual(movingPlatform.Position.X, lvl.Player.Position.X, delta);
                Assert.AreEqual(lvl.Player.CollideRectangle.Bottom, movingPlatform.CollideRectangle.Top, delta);
            }
        }

        [TestCaseSource(nameof(platformMoveCases))]
        public void Platform_Returns(float startX, float startY, float endX, float endY)
        {
            var lvl = new Level(clientSize.Size);
            const int velocity = 3;
            var movingPlatform = new MovingPlatform(new PointF(startX, startY), new PointF(endX, endY), velocity,
                1).CreatePlatform()[0];
            lvl.AddObject(movingPlatform);
            game.LoadLevel(() => lvl);
            CheckPlatformTrajectory(startX, startY, endX, endY, movingPlatform, velocity);
        }

        private void CheckPlatformTrajectory(float startX, float startY, float endX, float endY, Square movingPlatform,
            int velocity)
        {
            var touchEnd = false;
            for (var i = 0; i < 500; i++)
            {
                game.CurrentLevel.Update();

                Assert.GreaterOrEqual(movingPlatform.Position.X, Math.Min(startX, endX) - velocity);
                Assert.LessOrEqual(movingPlatform.Position.X, Math.Max(startX, endX) + velocity);
                Assert.GreaterOrEqual(movingPlatform.Position.Y, Math.Min(startY, endY) - velocity);
                Assert.LessOrEqual(movingPlatform.Position.Y, Math.Max(startY, endY) + velocity);
                if (Math.Abs(movingPlatform.Position.X - endX) <= velocity
                    && Math.Abs(movingPlatform.Position.X - endX) <= velocity)
                    touchEnd = true;
            }
            Assert.IsTrue(touchEnd);
        }

        private Square AddPlayerOnMovingPlatform(Level lvl, PointF startPosition, PointF endPosition)
        {
            var movingPlatform =
                new MovingPlatform(new PointF(startPosition.X, startPosition.Y), endPosition, 3,
                    1).CreatePlatform()[0];
            lvl.Player.Position =
                startPosition - new PointF(0, lvl.Player.CollideRectangle.Height + player.ColliderArea.Y);
            lvl.AddObject(movingPlatform);
            return movingPlatform;
        }

        private void UpdateLevel(Level level, int ticks)
        {
            for (var i = 0; i < ticks; i++)
                level.Update();
        }

        private int GetSing(float number) =>
            (int) (Math.Abs(number) / number);
    }
}
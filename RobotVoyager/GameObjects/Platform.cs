using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RobotVoyager.Engine;
using PointF = RobotVoyager.Engine.PointF;

namespace RobotVoyager.GameObjects
{
    internal class Platform
    {
        protected readonly List<Square> squares = new List<Square>();

        public Platform(PointF position, int length)
        {
            if (length < 1)
                throw new ArgumentException("length must be greater than 1");
            switch (length)
            {
                case 1:
                    squares.Add(SquareBuilder("sprites/Tiles/Tile (15).png", position));
                    break;
                case 2:
                    squares.Add(SquareBuilder("sprites/Tiles/Tile (12).png", position));
                    squares.Add(SquareBuilder("sprites/Tiles/Tile (14).png", new PointF(position.X + 128, position.Y)));
                    break;
                default:
                    squares.Add(SquareBuilder("sprites/Tiles/Tile (12).png", position));
                    for (var i = 1; i < length - 1; i++)
                        squares.Add(SquareBuilder("sprites/Tiles/Tile (13).png",
                            new PointF(position.X + 128 * i, position.Y)));
                    squares.Add(SquareBuilder("sprites/Tiles/Tile (14).png",
                        new PointF(position.X + 128 * (length - 1), position.Y)));
                    break;
            }
        }

        private static Square SquareBuilder(string fileName, PointF position)
        {
            return new Square(new Rectangle(position, new Size(128, 128)),
                new Bitmap(fileName)) { ColliderArea = new RectangleF(0, 0, 128, 64), MovableStatic = true };
        }

        public List<Square> CreatePlatform()
        {
            return squares.ToList();
        }
    }
}
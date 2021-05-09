using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CtrlZ.Engine
{
    internal class Sprite : Transform, ITexture, ISprite
    {
        public Sprite(RectangleF rectangle) : this(rectangle, new Bitmap("sprites/default.jpg")) { }

        public Sprite(RectangleF rectangle, Bitmap bmp) : base(rectangle.Location)
        {
            Image = ResizeImage(bmp, (int) rectangle.Width, (int) rectangle.Height);
        }

        public Sprite GetSprite()
        {
            return this;
        }

        public bool Visibility { get; set; } = true;
        public int ZIndex { get; set; }

        public virtual Image Image { get; set; }
        public bool ReversY { get; set; } = false;

        public static Bitmap ResizeImage(Image image, float scale)
        {
            return ResizeImage(image, (int) (image.Width * scale), (int) (image.Height * scale));
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            if (width == 0 || height == 0)
                return new Bitmap(1, 1);
            var destRect = new Rectangle(0, 0, width, height);

            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }

                #if DEBUG
                graphics.DrawRectangle(new Pen(Color.GreenYellow, 3), 0, 0, destImage.Width, destImage.Height);
                #endif
            }
            return destImage;
        }
    }
}
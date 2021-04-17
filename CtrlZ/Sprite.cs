using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CtrlZ
{
    class Sprite : Transform, ITexture, ISprite
    {
        public Sprite(RectangleF rectangle) : this(rectangle, new Bitmap("sprites/default.jpg")) { }
        public Sprite(RectangleF rectangle, Bitmap bmp) : base(rectangle.Location)
        {
            Image = ResizeImage(bmp, (int) rectangle.Width, (int) rectangle.Height);
        }

        public int ZIndex { get; set; }

        private Image image;
        public virtual Image Image
        {
            get => image;
            set
            {
                #if DEBUG
                using (var graphics = Graphics.FromImage(value))
                {
                    graphics.DrawRectangle(new Pen(Color.GreenYellow, 3), 0, 0, value.Width, value.Height);
                }
                #endif
                image = value;
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
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
            }

            return destImage;
        }

        public Sprite GetSprite()
        {
            return this;
        }
    }
}
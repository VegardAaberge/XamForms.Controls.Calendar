using System;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace XamForms.Controls.Droid
{
    public class ImageDrawable : Drawable
    {
        float ImageWidth { get; set; }

        float ImageHeight { get; set; }

        public bool IsSelected { get; set; }

        public float BorderWidth { get; set; }

        public Color BorderColor { get; set; }

        public Bitmap Bitmap { get; set; }

        public float Density { get; set; }

        public float ImagePadding { get; set; }

        public override int Opacity => 1;

        public override void Draw(Canvas canvas)
        {
            SetDimensions(canvas);

            var heightOffset = (canvas.Height - ImageHeight) / 2;
            var widthOffset = (canvas.Width - ImageWidth) / 2;

            var destRectF = new RectF
            {
                Top = heightOffset,
                Bottom = ImageHeight + heightOffset,
                Left = widthOffset,
                Right = ImageWidth + widthOffset
            };

            canvas.DrawBitmap(Bitmap, null, destRectF, null);

            if (IsSelected)
            {
                var paint = new Paint(PaintFlags.AntiAlias) { Color = BorderColor };
                paint.SetStyle(Paint.Style.Stroke);

                paint.StrokeWidth = BorderWidth * Density * 2;

                var rectF = new RectF
                {
                    Top = 0,
                    Bottom = canvas.Height,
                    Left = 0,
                    Right = canvas.Width
                };

                canvas.DrawRect(rectF, paint);
            }
        }

        void SetDimensions(Canvas canvas)
        {
            var yRatio = (float)Bitmap.Height / canvas.Height;
            var xRatio = (float)Bitmap.Width / canvas.Width;

            if (yRatio > xRatio)
            {
                ImageHeight = canvas.Height * (1 - ImagePadding);
                ImageWidth = Bitmap.Width * (ImageHeight / Bitmap.Height);
            }
            else
            {
                ImageWidth = canvas.Width * (1 - ImagePadding);
                ImageHeight = Bitmap.Height * (ImageWidth / Bitmap.Width);
            }
        }

        public override void SetAlpha(int alpha)
        {
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
        }
    }
}

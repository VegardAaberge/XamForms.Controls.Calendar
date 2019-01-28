using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Xamarin.Forms.Platform.Android;

namespace XamForms.Controls.Droid
{
    public class CustomDrawable : Drawable
    {
        public bool IsSelected { get; set; }

        public float BorderWidth { get; set; }

        public Color BorderColor { get; set; }

        public List<Pattern> Patterns = new List<Pattern>();

        public List<Circles> Circles = new List<Circles>();

        public override int Opacity => 1;

        public float Density { get; set; }

        public override void Draw(Canvas canvas)
        {
            float startHeight = 0;
            foreach (var item in Patterns)
            {
                var height = item.HeightPercent * canvas.Height;

                var paint = new Paint(PaintFlags.AntiAlias) { Color = item.Color.ToAndroid() };

                var patternRect = new RectF
                {
                    Top = startHeight,
                    Bottom = startHeight + height,
                    Left = 0,
                    Right = canvas.Width
                };

                canvas.DrawRect(patternRect, paint);

                startHeight += height;
            }

            foreach (var item in Circles)
            {
                var innerCirclePaint = new Paint(PaintFlags.AntiAlias) { Color = item.Color.ToAndroid() };
                var innerCircleWhitePaint = new Paint(PaintFlags.AntiAlias) { Color = Color.White };
                var outerCirclePaint = new Paint(PaintFlags.AntiAlias) { Color = item.BorderColor.ToAndroid() };
                outerCirclePaint.SetStyle(Paint.Style.Stroke);

                var strokeWidth = item.BorderThickness * Density;
                outerCirclePaint.StrokeWidth = strokeWidth;

                float cx = item.RelativeX * canvas.Width;
                float cy = item.RelativeY * canvas.Height;

                // Draw the outer circle
                canvas.DrawCircle(cx, cy, item.Radius + strokeWidth / 2, outerCirclePaint);
                canvas.DrawCircle(cx, cy, item.Radius, outerCirclePaint);

                var ovarRectF = new RectF
                {
                    Top = cy - item.Radius,
                    Bottom = cy + item.Radius,
                    Left = cx - item.Radius,
                    Right = cx + item.Radius,
                };

                // Draw the inner arc
                var angle = item.Percentage * 360;
                canvas.DrawCircle(cx, cy, item.Radius, innerCircleWhitePaint);
                canvas.DrawArc(ovarRectF, 270, angle, true, innerCirclePaint);

            }

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

        public override void SetAlpha(int alpha)
        {
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
        }
    }
}

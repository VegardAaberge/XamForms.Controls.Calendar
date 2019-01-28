using Android.Graphics.Drawables;
using XamForms.Controls.Droid;
using Xamarin.Forms.Platform.Android;
using XamForms.Controls;
using Android.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Xamarin.Forms;
using System;
using Android.Util;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarButton), typeof(CalendarButtonRenderer))]
namespace XamForms.Controls.Droid
{
    #pragma warning disable CS0618 // Type or member is obsolete
    [Preserve(AllMembers = true)]
    public class CalendarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;
            Control.TextChanged += (sender, a) =>
            {
                var element = Element as CalendarButton;
                if (Control.Text == element.TextWithoutMeasure || (string.IsNullOrEmpty(Control.Text) && string.IsNullOrEmpty(element.TextWithoutMeasure))) return;
                Control.Text = element.TextWithoutMeasure;
            };
            Control.SetPadding(1, 1, 1, 1);
            Control.ViewTreeObserver.GlobalLayout += (sender, args) => ChangeBackgroundPattern();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = Element as CalendarButton;

            if (element.BackgroundPattern != null && element.BackgroundPattern.Circles.Count > 0)
            {
            }

            if (e.PropertyName == nameof(element.TextWithoutMeasure) || e.PropertyName == "Renderer")
            {
                Control.Text = element.TextWithoutMeasure;
            }

            if (e.PropertyName == nameof(Element.TextColor) || e.PropertyName == "Renderer")
            {
                Control.SetTextColor(Element.TextColor.ToAndroid());
            }

            if (e.PropertyName == nameof(Element.BorderWidth) || e.PropertyName == nameof(Element.BorderColor) || e.PropertyName == nameof(Element.BackgroundColor) || e.PropertyName == "Renderer")
            {
                if (element.BackgroundPattern == null)
                {
                    if (element.BackgroundImage == null)
                    {
                        var drawable = new GradientDrawable();
                        drawable.SetShape(ShapeType.Rectangle);
                        var borderWidth = (int)Math.Ceiling(Element.BorderWidth);
                        drawable.SetStroke(borderWidth > 0 ? borderWidth + 1 : borderWidth, Element.BorderColor.ToAndroid());
                        drawable.SetColor(Element.BackgroundColor.ToAndroid());
                        Control.SetBackground(drawable);
                    }
                    else
                    {
                        ChangeBackgroundImage();
                    }
                }
                else
                {
                    ChangeBackgroundPattern();
                }
            }

            if (e.PropertyName == nameof(element.BackgroundPattern))
            {
                ChangeBackgroundPattern();
            }

            if (e.PropertyName == nameof(element.BackgroundImage))
            {
                ChangeBackgroundImage();
            }
        }

        protected async void ChangeBackgroundImage()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundImage == null) return;

            await Task.Yield();

            var image = await GetImageFromImageSource(element.BackgroundImage);

            var imageDrawable = new ImageDrawable
            {
                Bitmap = image,
                IsSelected = element.IsSelected,
                BorderWidth = (float)element.BorderWidth,
                BorderColor = element.BorderColor.ToAndroid(),
                Density = Resources.DisplayMetrics.Density,
                ImagePadding = (float)element.ImagePadding
            };

            Control.SetBackground(imageDrawable);
        }

        Bitmap ScaleBitmap(Bitmap image, double imagePadding)
        {
            var height = Control.Height * Resources.DisplayMetrics.Density;
            var width = Control.Width * Resources.DisplayMetrics.Density;

            var yRatio = (float)height / image.Height;
            var xRatio = (float)width / image.Width;

            var ratio = Math.Min(xRatio, yRatio);
            var paddingMultiplier = 1 / (1 - imagePadding);

            int dstWidth = (int)Math.Floor(image.Width * (ratio / paddingMultiplier));
            int dstHeight = (int)Math.Floor(image.Height * (ratio / paddingMultiplier));

            return Bitmap.CreateScaledBitmap(image, dstWidth, dstHeight, false);
        }

        protected async void ChangeBackgroundPattern()
		{
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundPattern == null || Control.Width == 0) return;

            await Task.Yield();

            CustomDrawable layer = new CustomDrawable
            {
                Patterns = element.BackgroundPattern.Pattern,
                Circles = element.BackgroundPattern.Circles,
                IsSelected = element.IsSelected,
                BorderColor = element.BorderColor.ToAndroid(),
                BorderWidth = (float)element.BorderWidth,
                Density = Resources.DisplayMetrics.Density,
            };

            Control.SetBackground(layer);
        }

        private async Task<Bitmap> GetImageFromImageSource(ImageSource imageSource)
        {
            IImageSourceHandler handler;

            if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler(); // sic
            }
            else if (imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler(); // sic
            }
            else
            {
                throw new NotImplementedException();
            }

            return await handler.LoadImageAsync(imageSource, this.Control.Context);
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public static class Calendar
	{
		public static void Init()
		{
			var d = "";
		}
	}

	public class TextDrawable : ColorDrawable
	{
		Paint paint;
		public Pattern Pattern { get; set; }

		public TextDrawable (Android.Graphics.Color color) 
			: base(color)
		{
			paint = new Paint();
			paint.AntiAlias = true;
			paint.SetStyle(Paint.Style.Fill);
			paint.TextAlign = Paint.Align.Left;
		}

		public override void Draw(Canvas canvas)
		{
			base.Draw(canvas);
			paint.Color = Pattern.TextColor.ToAndroid();
			paint.TextSize = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Sp,Pattern.TextSize > 0 ? Pattern.TextSize : 12,Forms.Context.Resources.DisplayMetrics);
			var bounds = new Rect();
			paint.GetTextBounds(Pattern.Text, 0, Pattern.Text.Length, bounds);
			var al = (int)Pattern.TextAlign;
			var x = Bounds.Left;
			if ((al & 2) == 2) // center
			{
				x = Bounds.CenterX() - Math.Abs(bounds.CenterX());
			} else if ((al & 4) == 4) // right
			{
				x = Bounds.Right - bounds.Width();
			}
			var y = Bounds.Top+Math.Abs(bounds.Top);
			if ((al & 16) == 16) // middle
			{
				y = Bounds.CenterY()+Math.Abs(bounds.CenterY());
			} else if ((al & 32) == 32) // bottom
			{
				y = Bounds.Bottom - Math.Abs(bounds.Bottom);
			}
			canvas.DrawText(Pattern.Text.ToCharArray(), 0, Pattern.Text.Length, x, y, paint);
		}
	}
}
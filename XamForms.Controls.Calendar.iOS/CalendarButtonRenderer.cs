﻿using XamForms.Controls;
using XamForms.Controls.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
#if __UNIFIED__
using Foundation;
#else
using MonoTouch.Foundation;
#endif

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarButton), typeof(CalendarButtonRenderer))]
namespace XamForms.Controls.iOS
{
    [Preserve(AllMembers = true)]
    public class CalendarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control == null)
                return;

            var element = Element as CalendarButton;
            if (e.PropertyName == nameof(element.TextWithoutMeasure) || e.PropertyName == "Renderer")
            {
                Control.SetTitle(element.TextWithoutMeasure, UIControlState.Normal);
                Control.SetTitle(element.TextWithoutMeasure, UIControlState.Disabled);
            }
            if (e.PropertyName == nameof(element.TextColor) || e.PropertyName == "Renderer")
            {
                Control.SetTitleColor(element.TextColor.ToUIColor(), UIControlState.Disabled);
                Control.SetTitleColor(element.TextColor.ToUIColor(), UIControlState.Normal);
            }
            if (e.PropertyName == nameof(element.BackgroundPattern))
            {
                DrawBackgroundPattern();
            }
            if (e.PropertyName == nameof(element.BackgroundImage))
            {
                DrawBackgroundImage();
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            Control.SetBackgroundImage(null, UIControlState.Normal);
            Control.SetBackgroundImage(null, UIControlState.Disabled);
            DrawBackgroundImage();
            DrawBackgroundPattern();
        }

        protected async void DrawBackgroundImage()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundImage == null) return;
            var image = await GetImage(element.BackgroundImage);

            var height = Control.Frame.Size.Height;
            var width = Control.Frame.Size.Width;
            nfloat ip = (nfloat)Math.Min(element.ImagePadding * height, element.ImagePadding * width);

            var imageView = new UIImageView(image);
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            imageView.Frame = new CGRect(ip / 2, ip / 2, Control.Frame.Size.Width - ip, Control.Frame.Size.Height - ip);

            Control.AddSubview(imageView);
            Control.BringSubviewToFront(imageView);
        }

        protected void DrawBackgroundPattern()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundPattern == null || Control.Frame.Width == 0) return;

            UIImage image;
            UIGraphics.BeginImageContext(Control.Frame.Size);
            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                for (var i = 0; i < element.BackgroundPattern.Pattern.Count; i++)
                {
                    var p = element.BackgroundPattern.Pattern[i];
                    g.SetFillColor(p.Color.ToCGColor());
                    var l = (int)Math.Ceiling(Control.Frame.Width * element.BackgroundPattern.GetLeft(i));
                    var t = (int)Math.Ceiling(Control.Frame.Height * element.BackgroundPattern.GetTop(i));
                    var w = (int)Math.Ceiling(Control.Frame.Width * element.BackgroundPattern.Pattern[i].WidthPercent);
                    var h = (int)Math.Ceiling(Control.Frame.Height * element.BackgroundPattern.Pattern[i].HeightPercent);
                    var r = new CGRect { X = l, Y = t, Width = w, Height = h };
                    g.FillRect(r);
                    DrawText(g, p, r);
                }

                for (var i = 0; i < element.BackgroundPattern.Circles.Count; i++)
                {
                    var item = element.BackgroundPattern.Circles[i];


                    var radius = item.Radius / UIScreen.MainScreen.Scale * 0.75f;
                    var x = (item.RelativeX * Control.Frame.Width);
                    var y = (item.RelativeY * Control.Frame.Height);

                    // Fill Arc
                    nfloat startRad = (nfloat)(270 * Math.PI / 180);
                    nfloat endRad = (nfloat)(startRad + item.Percentage * 360 * Math.PI / 180);

                    var center = new CGPoint(x, y);
                    var path = new UIBezierPath();
                    path.MoveTo(center);
                    path.AddArc(center, radius, startRad, endRad, true);
                    path.ClosePath();

                    g.SetFillColor(item.Color.ToCGColor());
                    path.Fill();

                    // Fill Rest Arc
                    center = new CGPoint(x, y);
                    path = new UIBezierPath();
                    path.MoveTo(center);
                    path.AddArc(center, radius, endRad, startRad, true);
                    path.ClosePath();

                    g.SetFillColor(new CGColor(255, 255, 255));
                    path.Fill();
                }

                image = UIGraphics.GetImageFromCurrentImageContext();
            }
            UIGraphics.EndImageContext();
            Control.SetBackgroundImage(image, UIControlState.Normal);
            Control.SetBackgroundImage(image, UIControlState.Disabled);
        }

        Task<UIImage> GetImage(ImageSource imageSource)
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

            return handler.LoadImageAsync(imageSource);
        }

        protected void DrawText(CGContext g, Pattern p, CGRect r)
        {
            if (string.IsNullOrEmpty(p.Text)) return;
            var bounds = p.Text.StringSize(UIFont.FromName("Helvetica", p.TextSize));
            var al = (int)p.TextAlign;
            var x = r.X; ;

            if ((al & 2) == 2) // center
            {
                x = r.X + (int)Math.Round(r.Width / 2.0) - (int)Math.Round(bounds.Width / 2.0);
            }
            else if ((al & 4) == 4) // right
            {
                x = (r.X + r.Width) - bounds.Width - 2;
            }
            var y = r.Y + (int)Math.Round(bounds.Height / 2.0) + 2;
            if ((al & 16) == 16) // middle
            {
                y = r.Y + (int)Math.Ceiling(r.Height / 2.0) + (int)Math.Round(bounds.Height / 5.0);
            }
            else if ((al & 32) == 32) // bottom
            {
                y = (r.Y + r.Height) - 2;
            }
            g.SaveState();
            g.TranslateCTM(0, Bounds.Height);
            g.ScaleCTM(1, -1);
            g.SetFillColor(p.TextColor.ToCGColor());
            g.SetTextDrawingMode(CGTextDrawingMode.Fill);
            g.SelectFont("Helvetica", p.TextSize, CGTextEncoding.MacRoman);
            g.ShowTextAtPoint(x, Bounds.Height - y, p.Text);
            g.RestoreState();
        }
    }

    public static class Calendar
    {
        public static void Init()
        {
            var d = "";
        }
    }
}


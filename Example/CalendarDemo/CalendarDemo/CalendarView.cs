using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XamForms.Controls;

namespace CalendarDemo
{
    public class CalendarView : ContentPage
    {
        public CalendarView()
        {
            var calendar = new Calendar
            {
                MaxDate = DateTime.Now.AddDays(30),
                MinDate = DateTime.Now.AddDays(-1),
                //DisableDatesLimitToMaxMinRange = true,
                MultiSelectDates = false,
                DisableAllDates = false,
                WeekdaysShow = true,
                ShowNumberOfWeek = true,
                //BorderWidth = 1,
                //BorderColor = Color.Transparent,
                //OuterBorderWidth = 0,
                //SelectedBorderWidth = 1,
                ShowNumOfMonths = 1,
                EnableTitleMonthYearView = true,
                WeekdaysTextColor = Color.Teal,
                StartDay = DayOfWeek.Monday,
                SelectedTextColor = Color.Fuchsia,
                SpecialDates = new List<SpecialDate>{
                    new SpecialDate(DateTime.Now.AddDays(2)) { BackgroundColor = Color.Green, TextColor = Color.Accent, BorderColor = Color.Lime, BorderWidth=8, Selectable = true },
                    new SpecialDate(DateTime.Now.AddDays(3))
                    {
                        BackgroundColor = Color.Green,
                        TextColor = Color.Blue,
                        Selectable = true,
                        BackgroundPattern = new BackgroundPattern(1)
                        {
                            Circles = new List<Circles>
                            {
                                new Circles { Color = Color.Lime, RelativeX=0.25f, RelativeY=0.75f, Radius=35, Percentage=0.4f, BorderThickness=1 },
                                new Circles { Color = Color.Red, RelativeX=0.75f, RelativeY=0.75f, Radius=35, Percentage=0.4f, BorderThickness=1 }
                            },
                            Pattern = new List<Pattern>
                            {
                                new Pattern{ WidthPercent = 1f, HeightPercent = 0.25f, Color = Color.Red},
                                new Pattern{ WidthPercent = 1f, HeightPercent = 0.25f, Color = Color.Purple},
                                new Pattern{ WidthPercent = 1f, HeightPercent = 0.25f, Color = Color.Green},
                                new Pattern{ WidthPercent = 1f, HeightPercent = 0.25f, Color = Color.Yellow,Text = "Test", TextColor=Color.DarkBlue, TextSize=11, TextAlign=TextAlign.Middle}
                            }
                        }
                    },
                    new SpecialDate(DateTime.Now.AddDays(4))
                    {
                        Selectable = true,
                        ImagePadding = 0.3,
                        BackgroundImage = ImageSource.FromFile("icon.png") as ImageSource
                    }
                }
            };

            calendar.DateClicked += (sender, e) => {
                System.Diagnostics.Debug.WriteLine(calendar.SelectedDates);
            };

            calendar.OnEndRenderCalendar += (object sender, EventArgs e) =>
            {
                (calendar.Content as StackLayout).Children.Last().HeightRequest = 500;
            };

            // The root page of your application
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 25 : 5, 5, 5),
                    Children = {
                        calendar
                    }
                }
            };
        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using XamForms.Controls;

namespace CalendarDemo
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class CalendarVM : BaseViewModel
	{
        public CalendarVM()
        {
            var specialDates = new List<SpecialDate>();
            specialDates.Add(
                new SpecialDate(DateTime.Now.AddDays(2)) { BackgroundColor = Color.Green, TextColor = Color.Accent, BorderColor = Color.Lime, BorderWidth = 8, Selectable = true }
            );

            specialDates.Add(
                new SpecialDate(DateTime.Now.AddDays(3))
                {
                    BackgroundColor = Color.Green,
                    TextColor = Color.Blue,
                    Selectable = true,
                    BackgroundPattern = new BackgroundPattern(1)
                    {
                        Pattern = new List<Pattern>
                        {
                            new Pattern{ WidthPercent = 1f, HightPercent = 0.25f, Color = Color.Red},
                            new Pattern{ WidthPercent = 1f, HightPercent = 0.25f, Color = Color.Purple},
                            new Pattern{ WidthPercent = 1f, HightPercent = 0.25f, Color = Color.Green},
                            new Pattern{ WidthPercent = 1f, HightPercent = 0.25f, Color = Color.Yellow,Text = "Test", TextColor=Color.DarkBlue, TextSize=11, TextAlign=TextAlign.Middle}
                        }
                    }
                }
            );

            specialDates.Add(
                new SpecialDate(DateTime.Now.AddDays(4))
                {
                    Selectable = true,
                    BackgroundImage = FileImageSource.FromFile("icon.png") as FileImageSource
                }
            );

            SpecialDates = new ObservableCollection<SpecialDate>(specialDates);
        }

		private ObservableCollection<SpecialDate> _specialDates;
		public ObservableCollection<SpecialDate> SpecialDates
		{
			get { return _specialDates; }
			set { _specialDates = value;  NotifyPropertyChanged(nameof(_specialDates));}
		}
	}
}

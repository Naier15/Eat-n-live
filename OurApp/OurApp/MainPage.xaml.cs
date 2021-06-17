using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using Xamarin.Forms;

namespace OurApp
{
	[DesignTimeVisible(true)]
	public partial class MainPage : ContentPage
	{
		protected int Count = 0;
		protected static DateTime Temp;
		protected static string Selected;
		protected NotificationRequest Notification;
		protected string Data;

		public class Alarmi
		{
			public string Time { get; set; }
			public string Name { get; set; }
			public string Method { get; set; }
			public Alarmi(DateTime Timee, string Namee, string Methody)
			{
				Time = Timee.ToString("HH:mm");
				Name = Namee;
				Method = Methody;
			}
		}

		public MainPage()
		{
			Title = "Главная страница";
			InitializeComponent();
			NotificationCenter.Current.NotificationReceived += NotiReceived;
			NotificationCenter.Current.NotificationTapped += OnLocalNotificationTapped;
		}

        protected override void OnAppearing()
        {
			Cali.SelectedDate = DateTime.Today;
			UpdateCalendar(new object(), new EventArgs());
			base.OnAppearing();
        }

		public void MyTask()
        { 
			NotificationCenter.Current.Cancel(int.Parse(Data)); 
		}

		public void NotiReceived(NotificationReceivedEventArgs E)
		{
			if (Count == 2)
            {
				Data = E.Data;
				App.ForAsync();
				Task Task = new Task(MyTask);
				
				App.DeleteAlarm2(int.Parse(E.Data));
				/*App.ShowAlarms();*/
				Count = 0;
				Task.Wait();
			} else
            {
				Count++; 
			}
		}

		private async void OnLocalNotificationTapped(NotificationTappedEventArgs E)
		{
			string Answer = await DisplayActionSheet("Время для таблеточки!", "Съел", "");
			if (Answer == "Съел")
            {
				NotificationCenter.Current.Cancel(int.Parse(E.Data));
				App.DeleteAlarm2(int.Parse(E.Data));
				/*App.ShowAlarms();*/
				Count = 0;
			}
		}

		protected static bool IsOn(App.Course Course, DateTime SelectedDay)
        {
			DateTime Finish;
			if (Course.Last != "1 год")
            {
				string[] Period = Course.Last.Split(new char[] { ' ' });
				Finish = Course.FirstDay.AddDays(Double.Parse(Period[0]));
			} 
			else { 
				Finish = Course.FirstDay.AddMonths(12); 
			}
			

			if (new DateTime(Course.FirstDay.Year, Course.FirstDay.Month, Course.FirstDay.Day) <= Temp 
				&& Temp < new DateTime(Finish.Year, Finish.Month, Finish.Day))
            {
				int Days;
				if (Course.Duration == "Через день")
                {
					Days = 2;
                } 
				else if (Course.Duration == "Раз в 3 дня")
                {
					Days = 3;
				}
				else if (Course.Duration == "1 раз в неделю")
                {
					Days = 7;
				} 
				else
                {
					return false;
                }

				if (new DateTime(Course.FirstDay.Year, Course.FirstDay.Month, Course.FirstDay.Day)
					== new DateTime(SelectedDay.Year, SelectedDay.Month, SelectedDay.Day) 
					|| Course.Duration == "Ежедневно") 
				{ 
					return true; 
				}
				else if (Days > 0)
				{
					for (DateTime Fd = Course.FirstDay; Fd < Finish; Fd = Fd.AddDays((double)Days)) 
					{ 
						if (Selected == Fd.ToString("dd.MM.yyyy")) 
						{ 
							return true; 
						} 
					}
				}
			} 
			return false;
        }

		public static List<Alarmi> ShowList()
		{
			List<Alarmi> Noti = new List<Alarmi> { };
			var Table = App.ShowCourses();
			if (Table.Count() > 0)
			{
				foreach (var Elem in Table)
                {
					if (IsOn(Elem, Temp))
					{
						if (Elem.Frequency >= 1) { Noti.Add(new Alarmi(Elem.FirstTime, Elem.Name, Elem.Method)); }
						if (Elem.Frequency >= 2) { Noti.Add(new Alarmi(Elem.SecondTime, Elem.Name, Elem.Method)); }
						if (Elem.Frequency >= 3) { Noti.Add(new Alarmi(Elem.ThirdTime, Elem.Name, Elem.Method)); }
						if (Elem.Frequency >= 4) { Noti.Add(new Alarmi(Elem.ForthTime, Elem.Name, Elem.Method)); }
					}
                }
				/*App.ShowAlarms();*/
                var NewList = from i in Noti orderby i.Time select i;
				return NewList.ToList();
			} else 
			{ 
				return null; 
			}
		}

		protected void ShowCalendar(object Sender, EventArgs E)
		{
			if (AbsoluteStack.IsVisible) 
			{ 
				AbsoluteStack.IsVisible = false; 
			} 
			else 
			{ 
				AbsoluteStack.IsVisible = true; 
			}
		}

		protected void UpdateCalendar(object Sender, EventArgs E)
        {
			DateTime Tmr = DateTime.Today.AddDays(1);

			try 
			{ 
				Temp = (DateTime)Cali.SelectedDate; 
			}
			catch 
			{ 
				Temp = DateTime.Today; 
			}

			Selected = Temp.ToString("dd.MM.yyyy");

			if (Selected == DateTime.Today.ToString("dd.MM.yyyy")) 
			{ 
				TodayBtn.Text = "Сегодня"; 
			} 
			else if (Selected == Tmr.ToString("dd.MM.yyyy"))  
			{ 
				TodayBtn.Text = "Завтра"; 
			} 
			else 
			{ 
				TodayBtn.Text = Selected; 
			}
			
			AbsoluteStack.IsVisible = false;
			ListPills.ItemsSource = ShowList();
			//App.ShowAlarms();
		}

		private void SkipToCourses(object Sender, EventArgs E) {
			Navigation.PushAsync(new CoursesPage());
        }
	}
}

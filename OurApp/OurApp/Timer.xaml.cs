using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OurApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Timer : ContentPage
    {
        public bool IsUpdating = false;
        public int Ind;
        public int C;
        public static Task Task;

        public Timer()
        {
            Title = "Задайте время приёма";
            InitializeComponent();
        }

        protected void SetNotific(App.Course Course, int Thread, int Id, DateTime Day, DateTime Time)
        {
            string Str = "";
            if (Course.Quantity != "Оставить пустым")
            { 
                Str += $"Принимать {Course.Quantity}"; 
            }

            string Str2 = "";
            if (Course.State != "Независимо")
            { 
                Str2 += $"- {Course.State}"; 
            }

            var Notification = new NotificationRequest
            {
                NotificationId = int.Parse($"{Course.Id}{Thread}{Id}"),
                Title = $"Время для {Course.Name}! {Str} {Str2}",
                Description = "Нажми чтобы отключить",
                NotifyTime = new DateTime(Day.Year, Day.Month, Day.Day, Time.Hour, Time.Minute, Time.Second),
                ReturningData = $"{Course.Id}{Thread}{Id}",
                Repeats = NotificationRepeat.TimeInterval,
                NotifyRepeatInterval = new TimeSpan(0, 10, 0),
                Android = new AndroidOptions()
                { 
                    AutoCancel = false, 
                    IconName = "pill" 
                },
            };

            App.InsertNewAlarm(new App.Alarm() 
            { 
                IdName = int.Parse($"{Course.Id}{Thread}{Id}"), 
                Ind = Course.Id 
            });
            NotificationCenter.Current.Show(Notification);
        }

        protected override void OnAppearing()
        {
            if (C == 1)
            {
                Tp1.Time = new TimeSpan(15, 0, 0);
                FirstTime.Children.Remove(Tp2);
                FirstTime.Children.Remove(Tp3);
                FirstTime.Children.Remove(Tp4);
            }
            else if (C == 2)
            {
                Tp1.Time = new TimeSpan(11, 0, 0);
                Tp2.Time = new TimeSpan(18, 0, 0);
                FirstTime.Children.Remove(Tp3);
                FirstTime.Children.Remove(Tp4);
            }
            else if (C == 3) 
            {
                Tp1.Time = new TimeSpan(8, 0, 0);
                Tp2.Time = new TimeSpan(15, 0, 0);
                Tp3.Time = new TimeSpan(22, 0, 0);
                FirstTime.Children.Remove(Tp4); 
            } else
            {
                Tp1.Time = new TimeSpan(8, 0, 0);
                Tp2.Time = new TimeSpan(11, 0, 0);
                Tp3.Time = new TimeSpan(15, 0, 0);
                Tp4.Time = new TimeSpan(22, 0, 0);
            }
            base.OnAppearing();
        }

        protected void SetNoti(App.Course Context)
        {
            int Step;
            if (Context.Duration == "Через день") 
            { 
                Step = 2; 
            }
            else if (Context.Duration == "Раз в 3 дня") 
            { 
                Step = 3; 
            }
            else if (Context.Duration == "1 раз в неделю") 
            { 
                Step = 7; 
            }
            else 
            { 
                Step = 1; 
            }

            DateTime Finish;
            if (Context.Last != "1 год")
            {
                string[] Period = Context.Last.Split(new char[] { ' ' });
                Finish = Context.FirstDay.AddDays(Double.Parse(Period[0]));
            } else 
            { 
                Finish = Context.FirstDay.AddMonths(12); 
            }

            int i = 1;
            for (DateTime Fd = Context.FirstDay; Fd < Finish; Fd = Fd.AddDays(Step))
            {
                if (Context.FirstTime != DateTime.MinValue)
                {
                    SetNotific(Context, 1, i, Fd, Context.FirstTime);
                }
                if (Context.SecondTime != DateTime.MinValue)
                {
                    SetNotific(Context, 2, i, Fd, Context.SecondTime);
                }
                if (Context.ThirdTime != DateTime.MinValue)
                {
                    SetNotific(Context, 3, i, Fd, Context.ThirdTime);
                }
                if (Context.ForthTime != DateTime.MinValue)
                {
                    SetNotific(Context, 4, i, Fd, Context.ForthTime);
                }
                i++;
            }
        }

        protected void CancelNotific(App.Course Context)
        {
            Task.Wait();
            var Rows = App.DeleteAlarm(Context.Id);
            foreach (var Elem in Rows)
            {
                NotificationCenter.Current.Cancel(Elem.IdName);
            }
            Task = Task.Run(() => SetNoti(Context));
        }

        private void Agree(object Sender, EventArgs E)
        {
            var Context = (App.Course)BindingContext;
            if (C >= 1) 
            { 
                Context.FirstTime = DateTime.Parse(Tp1.Time.ToString()); 
            }
            if (C >= 2) 
            { 
                Context.SecondTime = DateTime.Parse(Tp2.Time.ToString()); 
            }
            if (C >= 3) 
            { 
                Context.ThirdTime = DateTime.Parse(Tp3.Time.ToString()); 
            }
            if (C >= 4) 
            { 
                Context.ForthTime = DateTime.Parse(Tp4.Time.ToString()); 
            }
            
            if (IsUpdating)
            {
                App.UpdateCourse(Context);
                Task.Run(() => CancelNotific(Context));
                /*App.ShowAlarms();*/
            } else
            {
                App.InsertNewCourse(Context);
                Task = Task.Run(() => SetNoti(Context));
            }
            Navigation.PopAsync();
            Navigation.PopAsync();
        }
    }
}
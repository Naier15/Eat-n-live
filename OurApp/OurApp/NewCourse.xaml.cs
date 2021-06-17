using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OurApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewCourse : ContentPage {
        public NewCourse() {
            Title = "Редактирование курса";
            InitializeComponent();
        }

        protected static int PrevFrequency;
        protected static string PrevDuration;
        protected static string PrevLast;
        protected static DateTime Date;

        protected override void OnAppearing()
        {
            var Temp = (App.Course)BindingContext;
            PrevDuration = Temp.Duration;
            PrevFrequency = Temp.Frequency;
            PrevLast = Temp.Last;
            base.OnAppearing();
        }

        private async void AcceptCourse(object sender, EventArgs e)
        {
            var Context = (App.Course)BindingContext;
            if (Context.Id != 0)
            {
                if (Context.Frequency != PrevFrequency 
                    || Context.Duration != PrevDuration 
                    || Context.Last != PrevLast)
                {
                    Timer Timer = new Timer() 
                    { 
                        C = Context.Frequency, 
                        Ind = Context.Id, 
                        IsUpdating=true 
                    };
                    Context.FirstTime = new DateTime();
                    Context.SecondTime = new DateTime();
                    Context.ThirdTime = new DateTime();
                    Context.ForthTime = new DateTime();
                    Timer.BindingContext = Context;

                    await this.Navigation.PushAsync(Timer, true);
                }
                else
                {
                    App.UpdateCourse(Context);
                    await this.Navigation.PopAsync();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Context.Name))
                {
                    CourseName.Placeholder = "Вы забыли указать название";
                    CourseName.PlaceholderColor = Color.Red;
                } 
                else if (Context.Frequency == 0 
                    || String.IsNullOrEmpty(Context.Method) 
                    || String.IsNullOrEmpty(Context.Quantity) 
                    || String.IsNullOrEmpty(Context.Duration) 
                    || String.IsNullOrEmpty(Context.State) 
                    || String.IsNullOrEmpty(Context.Last))
                { 
                    lbl.Text = "Заполните все строки, пожалуйста"; 
                } 
                else
                {
                    Timer Timer = new Timer() 
                    { 
                        C = Context.Frequency, 
                        Ind = Context.Id 
                    };
                    Timer.BindingContext = Context;

                    await this.Navigation.PushAsync(Timer, true);
                }
            }
        }
    }

    public class Freq_converter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        { 
            return Value.ToString(); 
        }
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        { 
            return int.Parse(Value.ToString()); 
        }
    }
}
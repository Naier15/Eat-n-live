using Plugin.LocalNotification;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace OurApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursesPage : ContentPage {
        public CoursesPage() {
            Title = "Список курсов";
            InitializeComponent();
        }

        public void Checking()
        {
            var Table = App.ShowCourses();
            if (Table.Count() > 0)
            {
                CourseList.ItemsSource = Table;
                StateStack.Children.Remove(ListState);
            }
            else
            {
                CourseList.ItemsSource = "";
                StateStack.Children.Add(ListState);
            }
        }

        protected override void OnAppearing() {
            Checking();
            base.OnAppearing();
        }

        private async void OnItemSelected(object Sender, SelectedItemChangedEventArgs E)
        {
            App.Course SelectedCourse = (App.Course)E.SelectedItem;
            NewCourse NewCourse = new NewCourse
            { 
                BindingContext = SelectedCourse 
            };
            await Navigation.PushAsync(NewCourse, true);
        }

        private void Cancel(int I)
        {
            try
            {
                Timer.Task.Wait();

                var Rows = App.DeleteAlarm(I);
                foreach (var Elem in Rows)
                {
                    NotificationCenter.Current.Cancel(Elem.IdName);
                }
            } catch
            {
                Console.WriteLine($"------- Что то в канселе пошло не так");
            }
            
        }

        private void Delete(Object Sender, EventArgs E)
        {
            var Item = (ImageButton)Sender;
            int I = int.Parse(Item.CommandParameter.ToString());
            App.DeleteCourse(I);
            Task.Run(() => Cancel(I));
            Checking();
        }

        private async void AddNewOne(object Sender, EventArgs E)
        {
            App.Course Course = new App.Course() 
            { 
                FirstDay = DateTime.Today 
            };
            NewCourse NewCourse = new NewCourse
            { 
                BindingContext = Course
            };
            await Navigation.PushAsync(NewCourse, true);
        }
    }

    public class FistDayConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            DateTime Dt = DateTime.Parse(Value.ToString());
            return "Начался " + Dt.ToString("dd.MM.yyyy"); 
        }
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        { 
            return DateTime.Parse(Value.ToString()); 
        }
    }
}
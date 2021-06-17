using System;
using Xamarin.Forms;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OurApp
{
    public partial class App : Application
	{

		[Table("Courses")]
		public class Course {
			[PrimaryKey, AutoIncrement, Column("id")]
			public int Id { get; set; }

			public string Name { get; set; }
			public string Method { get; set; }
			public string Quantity { get; set; }
			public int Frequency { get; set; }
			public string Duration { get; set; }
			public string State { get; set; }
			public string Last { get; set; }
			public DateTime FirstDay { get; set; }
			public DateTime FirstTime { get; set; }
			public DateTime SecondTime { get; set; }
			public DateTime ThirdTime { get; set; }
			public DateTime ForthTime { get; set; }
		}

		[Table("Alarms")]
		public class Alarm
		{
			[PrimaryKey, AutoIncrement, Column("id")]
			public int Id { get; set; }

			public int IdName { get; set; }
			public int Ind { get; set; }
		}

		public static SQLiteConnection DataPath()
        {
			string dbPath = Path.Combine(
				 Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				 "db.db");
			var Db = new SQLiteConnection(dbPath);
			Db.CreateTable<Course>();
			return Db;
		}

		public async static void ForAsync()
		{
			string DbPath = Path.Combine(
				 Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				 "db.db");
			var Db = new SQLiteAsyncConnection(DbPath);
            try 
			{ 
				var e = await Db.Table<Course>().ToListAsync();
                await Db.CloseAsync();
			} 
			catch 
			{ 
				Console.WriteLine("---------------- Что-то пошло не так ForAsync"); 
			}
		}

		public static SQLiteConnection AlarmsPath()
		{
			string DbPath = Path.Combine(
				 Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				 "db1.db");
			var Db = new SQLiteConnection(DbPath);
			Db.CreateTable<Alarm>();
			return Db;
		}

		public static void InsertNewAlarm(Alarm Context)
		{
			var Db = AlarmsPath();
			Db.Insert(Context);
			Db.Close();
		}

		public static List<Alarm> DeleteAlarm(int Id)
		{
			List<Alarm> Rows = new List<Alarm>();
            try
            {
				var Db = AlarmsPath();
				Rows = Db.Query<Alarm>($"SELECT IdName FROM Alarms WHERE Ind={Id}");
				Db.Query<Alarm>($"DELETE FROM Alarms WHERE Ind={Id};");
				Db.Close();
			} 
			catch 
			{ 
				Console.WriteLine("-------> Что-то пошло не так DeleteAlarm"); 
			}
			return Rows;
		}

		public static void DeleteAlarm2(int Id)
		{
            try
            {
				var Db = AlarmsPath();
				Db.Query<Alarm>($"DELETE FROM Alarms WHERE IdName={Id};");
				Db.Close();
			} catch { 
				Console.WriteLine("-----> Что-то пошло не так DeleteAlarm2"); 
			}
			
		}

		/*public static void ShowAlarms()
        {
            try {
				var db = AlarmsPath();
				var Alarms = db.Table<Alarm>();
				if (Alarms.Count() > 0)
				{
					int k = 0;
					foreach (var elem in Alarms)
					{
						Console.WriteLine($"===== {elem.Id} {elem.IdName} {elem.Ind}");
						k++;
					}
					Console.WriteLine($"===== total count {k}");
				}
				else { Console.WriteLine("---------------- Нет в базе уведомлений"); }
			} catch { Console.WriteLine("---------------- Что-то пошло не так ShowAlarms"); }
		}*/

		public static void InsertNewCourse(Course Context)
		{
			var Db = DataPath();
			Db.Insert(Context);
			Db.Close();
		}

		public async static void UpdateCourse(Course Context)
		{
			var Db = await Task.Run(() => DataPath());
			Db.Update(Context);
			Db.Close();
		}

		public static TableQuery<Course> ShowCourses()
        {
			var Db = DataPath();
			var Table = Db.Table<Course>();
			return Table;
		}

		public static void DeleteCourse(int Id)
        {
			var Db = DataPath();
			Db.Delete<Course>(Id);
			Db.Close();
		}


		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new MainPage());
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}

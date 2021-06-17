using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Platform.Droid;
using Android.Content;

namespace OurApp.Droid
{
	[Activity(Label = "Жри и живи", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			NotificationCenter.CreateNotificationChannel(
				new NotificationChannelRequest {
					EnableLights = true,
					EnableVibration = true,
					LightColor = Android.Graphics.Color.White,
					Importance = NotificationImportance.Max,
					LockscreenVisibility = NotificationVisibility.Public,
					Name = "Important"
					});

			LoadApplication(new App());

			NotificationCenter.NotifyNotificationTapped(Intent);
		}

		protected override void OnNewIntent(Intent intent)
		{
			NotificationCenter.NotifyNotificationTapped(intent);
			base.OnNewIntent(intent);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}
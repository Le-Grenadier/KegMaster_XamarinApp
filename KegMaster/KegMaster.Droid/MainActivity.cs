using Android.App;
using Android.Content.PM;
using Microsoft.Identity.Client;
using Android.Content;
using Android.OS;

using KegMaster.Core;
using KegMaster.Core.Features.LogOn;

using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace KegMaster.Droid
{
    [Activity(Label = "KegMaster", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
		public const string TAG = "MainActivity";
		internal static readonly string CHANNEL_ID = "KegMaster_NotificationHub";

		protected override void OnCreate(Bundle bundle)
        {
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // Initialize Azure Mobile Apps
            CurrentPlatform.Init();
            LoadApplication(new App());

            // Initialize Xamarin Forms
            Forms.Init(this, bundle);

            var authenticationService = DependencyService.Get<IAuthenticationService>();
            // Default system browser
            authenticationService.SetParent(this);

            // Acomodate keyboards
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
            AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }


    }
}


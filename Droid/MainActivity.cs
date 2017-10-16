using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using dona.Droid.Services;
using Plugin.FirebasePushNotification;
using Plugin.Permissions;

namespace dona.Droid
{
    [Activity(
        Label = "Doná",
        Icon = "@drawable/navbar",
        Theme = "@style/MyTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly ReceiveSmsBroadcastReceiver _receiveSmsBroadcastReceiver = new ReceiveSmsBroadcastReceiver();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new dona.Forms.App());

            // initialize Firebase Push Notifications
            FirebasePushNotificationManager.ProcessIntent(Intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(_receiveSmsBroadcastReceiver, new IntentFilter("SMS_RECEIVED"));
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(_receiveSmsBroadcastReceiver);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
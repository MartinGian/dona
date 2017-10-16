using Android.App;
using Android.OS;
using Android.Support.V7.App;
using dona.Forms.Services;

namespace dona.Droid
{
    [Activity(Label = "Doná", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            await InstitutionsService.Instance.Initialize();
            StartActivity(typeof(MainActivity));
        }
    }
}
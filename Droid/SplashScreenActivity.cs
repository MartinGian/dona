using Android.App;
using Android.OS;
using Android.Support.V7.App;
using dona.Forms.Services;
using System.Threading.Tasks;
using Android.Content;

namespace dona.Droid
{
    [Activity(Label = "Doná", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            await Task.Run(async () => await InstitutionsService.Instance.Initialize());

            var startActivityIntent = new Intent(this, typeof(MainActivity));
            StartActivity(startActivityIntent);
            Finish();
        }

        // To prevent the back button from canceling the startup process
        public override void OnBackPressed() { }
    }
}
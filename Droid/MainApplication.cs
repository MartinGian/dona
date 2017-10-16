using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.FirebasePushNotification;
using Plugin.CurrentActivity;
using Microsoft.WindowsAzure.MobileServices;
using Acr.UserDialogs;
using dona.Droid.Services;
using dona.Forms.Dependencies;
using FFImageLoading.Forms.Droid;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace dona.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
              FirebasePushNotificationManager.Initialize(this,false);
#endif

            FirebasePushNotificationManager.NotificationContentTitleKey = "donaNotificationTitle";
            FirebasePushNotificationManager.NotificationContentTextKey = "donaNotificationMessage";

            RegisterActivityLifecycleCallbacks(this);

            CurrentPlatform.Init();
            CachedImageRenderer.Init();
            UserDialogs.Init(this);
            DependencyService.Register<INetworkInformation, NetworkInformation>();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {

        }

        public void OnActivityPaused(Activity activity)
        {

        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {

        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {

        }
    }
}
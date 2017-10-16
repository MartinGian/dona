using Android.App;
using Android.Telephony;
using dona.Forms.Dependencies;

namespace dona.Droid.Services
{
    public class NetworkInformation : INetworkInformation
    {
        public string GetNetworkOperatorName()
        {
            var telephonyManager = TelephonyManager.FromContext(Application.Context);
            return telephonyManager.NetworkOperatorName;
        }
    }
}
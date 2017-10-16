using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;
using dona.Forms.Events;
using Xamarin.Forms;

namespace dona.Droid.Services
{
    [BroadcastReceiver(Exported = true)]
    [IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class ReceiveSmsBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != IntentAction) return;

                var bundle = intent.Extras;

                if (bundle != null)
                {
                    var pdus = (Java.Lang.Object[])bundle.Get("pdus");
                    var messages = new SmsMessage[pdus.Length];
                    var messageBuilder = new StringBuilder();
                    for (var i = 0; i < messages.Length; i++)
                    {
                        messages[i] = SmsMessage.CreateFromPdu((byte[])pdus[i]);
                        messageBuilder.AppendFormat("Message from {0} here is the Content : {1}",
                            messages[i].OriginatingAddress, messages[i].MessageBody);
                    }

                    InvokeAbortBroadcast();

                    Task.Run(() => MessagingCenter.Send(
                        new CreditInformationReceivedEvent
                        {
                            Message = messageBuilder.ToString(),
                            Number = messages[0].OriginatingAddress,
                            Success = true
                        }, "SmsMessageReceived"));
                }
            }
            catch (Exception)
            {
                Task.Run(() => MessagingCenter.Send(new CreditInformationReceivedEvent { Success = false }, "SmsMessageReceived"));
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using dona.Forms.Exceptions;
using Plugin.Messaging;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace dona.Forms.Services
{
    public interface ISmsService
    {
        /// <summary>
        /// Sends sms to the number with the message received. If the app don't have permission to send sms, it request it.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        Task SendSmsAsync(string number, string message);
    }

    public class SmsService : ISmsService
    {
        public async Task SendSmsAsync(string number, string message)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentNullException(nameof(number));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            PermissionStatus permissionStatus;
            if (CrossPermissions.IsSupported)
            {
                permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sms);
                if (permissionStatus != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Sms))
                        await UserDialogs.Instance.AlertAsync("Para proceder, es necesario que Doná tenga permisos para enviar SMS en su teléfono. Tendrá que darle permisos por única vez.", "Permisos");

                    var requestPermissionsResult = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Sms);
                    if (requestPermissionsResult.ContainsKey(Permission.Sms))
                        permissionStatus = requestPermissionsResult[Permission.Sms];
                }
            }
            else permissionStatus = PermissionStatus.Granted;

            if (permissionStatus == PermissionStatus.Granted)
                if (CrossMessaging.Current.SmsMessenger.CanSendSmsInBackground)
                    CrossMessaging.Current.SmsMessenger.SendSmsInBackground(number, message);
                else
                    throw new CannotSendSmsException("No es posible enviar SMS");
            else
                throw new CannotSendSmsException("La aplicación no tiene permisos para enviar SMS");
        }
    }
}

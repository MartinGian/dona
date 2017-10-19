using System;
using Xamarin.Forms;

namespace dona.Forms.Helpers
{
    public static class UiHelper
    {
        public static void InvokeOnTheMainThread(Action action)
        {
            if (Device.IsInvokeRequired)
                Device.BeginInvokeOnMainThread(action);
            else
                action();
        }
    }
}

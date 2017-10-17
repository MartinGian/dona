using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace MyApp.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Net.ConnectivityManager.ConnectivityAction, Android.Net.Wifi.WifiManager.NetworkStateChangedAction)]
    public class NetworkBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            var connManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            NetworkInfo mWifi = connManager.GetNetworkInfo(ConnectivityType.Wifi);
            if (mWifi.IsConnected)
            {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context)
                .SetContentTitle("Wifi is connected")
                .SetContentText("Open your app to sync your data!");

                // Build the notification:
                var notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
        }
    }
}
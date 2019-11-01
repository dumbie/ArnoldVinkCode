using ArnoldVinkCode;
using Foundation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AVNotifications))]
namespace ArnoldVinkCode
{
    public class AVNotifications : ArnoldVinkNotifications
    {
        //Send Notification
        public async Task SendNotification(string Title, string Text)
        {
            try
            {
                UILocalNotification notification = new UILocalNotification();

                notification.AlertAction = Title;
                notification.AlertBody = Text;

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);

                Debug.WriteLine("Sended notification: " + Text);
            }
            catch { Debug.WriteLine("Failed sending notification: " + Text); }
        }
    }
}
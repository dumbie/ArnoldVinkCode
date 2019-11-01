using Android;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using ArnoldVinkCode;
using System.Diagnostics;
using System.Threading.Tasks;
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
                Context FormsContext = Forms.Context;
                NotificationCompat.Builder builder = new NotificationCompat.Builder(FormsContext).SetContentTitle(Title).SetContentText(Text).SetSmallIcon(Resource.Drawable.ButtonDefault);
                Notification notification = builder.Build();
                NotificationManager notificationManager = FormsContext.GetSystemService(Context.NotificationService) as NotificationManager;
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
            catch { Debug.WriteLine("Failed sending notification: " + Text); }
        }
    }
}
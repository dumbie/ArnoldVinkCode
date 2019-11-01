using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public interface ArnoldVinkNotifications
    {
        //Send Notification
         Task SendNotification(string Title, string Text);
    }
}
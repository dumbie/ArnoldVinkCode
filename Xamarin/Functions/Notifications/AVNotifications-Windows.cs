using ArnoldVinkCode;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
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
                XmlDocument Tile_XmlContent = new XmlDocument();
                ToastNotifier Toast_UpdateManager = ToastNotificationManager.CreateToastNotifier();

                Tile_XmlContent.LoadXml("<toast><visual><binding template=\"ToastImageAndText03\"><image id=\"1\" src=\"ms-appx:///Assets/LargeTile.scale-100.png\"/><text id=\"1\">" + WebUtility.HtmlEncode(Title) + "</text><text id=\"2\">" + WebUtility.HtmlEncode(Text) + "</text></binding></visual></toast>");
                Toast_UpdateManager.Show(new ToastNotification(Tile_XmlContent) { SuppressPopup = false, Tag = "T1", Group = "G1" });
            }
            catch { Debug.WriteLine("Failed sending notification: " + Text); }
        }
    }
}
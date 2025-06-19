using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArnoldVinkCode.Styles
{
    public partial class ImageRound : UserControl
    {
        //Variables
        public ImageSource Source
        {
            get
            {
                return image_Round.Source;
            }
            set
            {
                image_Round.Source = value;
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return border_Round.CornerRadius;
            }
            set
            {
                border_Round.CornerRadius = value;
            }
        }

        //Initialize
        public ImageRound()
        {
            try
            {
                InitializeComponent();
            }
            catch { }
        }
    }
}
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ArnoldVinkStyles
{
    //Import:
    //xmlns:AVStyles="clr-namespace:ArnoldVinkStyles"
    //Usage:
    //<AVStyles:ScrollViewerLoopVertical ScrollLoopSpeed="60" ScrollLoopStep="0.50"/>
    //<AVStyles:ScrollViewerLoopHorizontal ScrollLoopSpeed="60" ScrollLoopStep="0.50"/>
    public class ScrollViewerLoopHorizontal : ScrollViewer
    {
        private bool MovingToEnding = false;
        public bool ScrollPaused { get; set; } = false;
        public int ScrollLoopSpeed { get; set; } = 120;
        public double ScrollLoopStep { get; set; } = 0.80;

        public async override void OnApplyTemplate()
        {
            try
            {
                while (true)
                {
                    //Check if the scroller is active
                    if (ScrollPaused || this.ScrollableWidth < 5)
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    //Check the scrollbar position
                    if (this.HorizontalOffset == this.ScrollableWidth) { MovingToEnding = false; }
                    else if (this.HorizontalOffset == 0) { MovingToEnding = true; }

                    //Scroll the scrollbar
                    if (MovingToEnding)
                    {
                        await Task.Delay(ScrollLoopSpeed);
                        this.ScrollToHorizontalOffset(this.HorizontalOffset + ScrollLoopStep);
                    }
                    else
                    {
                        await Task.Delay(ScrollLoopSpeed);
                        this.ScrollToHorizontalOffset(this.HorizontalOffset - ScrollLoopStep);
                    }
                }
            }
            catch { }
        }
    }

    public class ScrollViewerLoopVertical : ScrollViewer
    {
        private bool MovingToEnding = false;
        public bool ScrollPaused { get; set; } = false;
        public int ScrollLoopSpeed { get; set; } = 120;
        public double ScrollLoopStep { get; set; } = 0.80;

        public async override void OnApplyTemplate()
        {
            try
            {
                while (true)
                {
                    //Check if the scroller is active
                    if (ScrollPaused || this.ScrollableHeight < 5)
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    //Check the scrollbar position
                    if (this.VerticalOffset == this.ScrollableHeight) { MovingToEnding = false; }
                    else if (this.VerticalOffset == 0) { MovingToEnding = true; }

                    //Scroll the scrollbar
                    if (MovingToEnding)
                    {
                        await Task.Delay(ScrollLoopSpeed);
                        this.ScrollToVerticalOffset(this.VerticalOffset + ScrollLoopStep);
                    }
                    else
                    {
                        await Task.Delay(ScrollLoopSpeed);
                        this.ScrollToVerticalOffset(this.VerticalOffset - ScrollLoopStep);
                    }
                }
            }
            catch { }
        }
    }
}
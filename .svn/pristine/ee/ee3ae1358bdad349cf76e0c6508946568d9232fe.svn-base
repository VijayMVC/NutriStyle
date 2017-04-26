using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls.Scroll
{
    public class ScrollDataGrid : DataGrid
    {
        /// <summary>
        /// The vertical scrollbar.
        /// </summary>
        private ScrollBar verticalScrollBar;

        /// <summary>
        /// The horizontal scrollbar.
        /// </summary>
        private ScrollBar horizontalScrollBar;

        /// <summary>
        /// Position of the vertical scrollbar we saved.
        /// </summary>
        private double savedVerticalScrollPosition;

        /// <summary>
        /// Position of the horizontal scrollbar we saved.
        /// </summary>
        private double savedHorizontalScrollPosition;

        /// <summary>
        /// Event for each vertical scroll.
        /// </summary>
        public event EventHandler<ScrollEventArgs> VerticalScroll;

        /// <summary>
        /// Event for each horizontal scroll.
        /// </summary>
        public event EventHandler<ScrollEventArgs> HorizontalScroll;

        /// <summary>
        /// Load the scrollbars after the template gets loaded.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LoadScrollBars();
        }

        /// <summary>
        /// Get both scrollbars.
        /// </summary>
        private void LoadScrollBars()
        {
            verticalScrollBar = this.GetTemplateChild("VerticalScrollbar") as ScrollBar;
            if (verticalScrollBar != null)
                verticalScrollBar.Scroll += new ScrollEventHandler(OnVerticalScroll);
            horizontalScrollBar = this.GetTemplateChild("HorizontalScrollbar") as ScrollBar;
            if (horizontalScrollBar != null)
                horizontalScrollBar.Scroll += new ScrollEventHandler(OnHorizontalScroll);
        }

        /// <summary>
        /// Notify that we are scrolling vertically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            if (VerticalScroll != null)
                VerticalScroll(sender, e);
        }

        /// <summary>
        /// Notify that we are scrolling horizontally.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHorizontalScroll(object sender, ScrollEventArgs e)
        {
            if (HorizontalScroll != null)
                HorizontalScroll(sender, e);
        }

        /// <summary>
        /// Save the current scroll position.
        /// </summary>
        /// <param name="mode"></param>
        public void SaveScrollPosition(ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    this.savedVerticalScrollPosition = verticalScrollBar.Value;
                    break;
                case ScrollMode.Horizontal:
                    this.savedHorizontalScrollPosition = horizontalScrollBar.Value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reload the scroll position that was saved before.
        /// </summary>
        /// <param name="mode"></param>
        public void ReloadScrollPosition(ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    this.Scroll(ScrollMode.Vertical, savedVerticalScrollPosition);
                    break;
                case ScrollMode.Horizontal:
                    this.Scroll(ScrollMode.Horizontal, savedHorizontalScrollPosition);
                    break;
            }
        }
    }
}

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
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls.Scroll
{
    public static class DataGridScrollExtensions
    {
        /// <summary>
        /// Scroll to the start of the ScrollBar.
        /// <param name="mode"></param>
        public static void ScrollToStart(this DataGrid grid, ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    grid.ScrollToPercent(ScrollMode.Vertical, 0);
                    break;
                case ScrollMode.Horizontal:
                    grid.ScrollToPercent(ScrollMode.Horizontal, 0);
                    break;
            }
        }

        /// <summary>
        /// Scroll to the end of the ScrollBar.
        /// </summary>
        /// <param name="mode"></param>
        public static void ScrollToEnd(this DataGrid grid, ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    grid.ScrollToPercent(ScrollMode.Vertical, 100);
                    break;
                case ScrollMode.Horizontal:
                    grid.ScrollToPercent(ScrollMode.Horizontal, 100);
                    break;
            }
        }

        /// <summary>
        /// Scroll to a percentage of the scrollbar (50% = half).
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="percent"></param>
        public static void ScrollToPercent(this DataGrid grid, ScrollMode mode, double percent)
        {
            // Fix the percentage.
            if (percent < 0)
                percent = 0;
            else if (percent > 100)
                percent = 100;

            // Get the scroll provider.
            var scrollProvider = GetScrollProvider(grid);

            // Scroll.
            switch (mode)
            {
                case ScrollMode.Vertical:
                    scrollProvider.SetScrollPercent(System.Windows.Automation.ScrollPatternIdentifiers.NoScroll, percent);
                    break;
                case ScrollMode.Horizontal:
                    scrollProvider.SetScrollPercent(percent, System.Windows.Automation.ScrollPatternIdentifiers.NoScroll);
                    break;
            }
        }

        /// <summary>
        /// Get the current position of the scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double GetScrollPosition(this DataGrid grid, ScrollMode mode)
        {
            var scrollBar = grid.GetScrollbar(mode);
            return scrollBar.Value;
        }

        /// <summary>
        /// Get the maximum position of a scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double GetScrollMaximum(this DataGrid grid, ScrollMode mode)
        {
            var scrollBar = grid.GetScrollbar(mode);
            return scrollBar.Maximum;
        }

        /// <summary>
        /// Scroll to a position of the scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="position"></param>
        public static void Scroll(this DataGrid grid, ScrollMode mode, double position)
        {
            // Get the scrollbar and convert the position to percent.
            var scrollBar = grid.GetScrollbar(mode);
            double positionPct = ((position / scrollBar.Maximum) * 100);

            // Scroll to a specfic percentage of the scrollbar.
            grid.ScrollToPercent(mode, positionPct);
        }

        /// <summary>
        /// Get a scroll provider for the grid.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static IScrollProvider GetScrollProvider(DataGrid grid)
        {
            var p = FrameworkElementAutomationPeer.FromElement(grid) ?? FrameworkElementAutomationPeer.CreatePeerForElement(grid);
            return p.GetPattern(PatternInterface.Scroll) as IScrollProvider;
        }

        /// <summary>
        /// Get one of the grid's scrollbars.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ScrollBar GetScrollbar(this DataGrid grid, ScrollMode mode)
        {
            if (mode == ScrollMode.Vertical)
                return grid.GetScrollbar("VerticalScrollbar");
            else
                return grid.GetScrollbar("HorizontalScrollbar");
        }

        /// <summary>
        /// Find the scrollbar for our datagrid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dep"></param>
        /// <returns></returns>
        private static ScrollBar GetScrollbar(this DependencyObject dep, string name)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if (child != null && child is ScrollBar && ((ScrollBar)child).Name == name)
                    return child as ScrollBar;
                else
                {
                    ScrollBar sub = GetScrollbar(child, name);
                    if (sub != null)
                        return sub;
                }
            }
            return null;
        }
    }
}

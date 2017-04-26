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
using System.Collections.Generic;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public static class VisualTreeEnumeration
    {
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                    yield return descendent;
            }
        }
    }
}

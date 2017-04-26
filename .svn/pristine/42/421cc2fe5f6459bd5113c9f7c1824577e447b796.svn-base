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

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public static partial class Extensions
    {
        public static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
        {

            while (obj != null)
            {

                T o = obj as T;

                if (o != null)

                    return o;

                obj = VisualTreeHelper.GetParent(obj);

            }

            return null;

        }



        public static T FindAncestor<T>(this UIElement obj) where T : UIElement
        {

            return FindAncestor<T>((DependencyObject)obj);

        }

 
    }
}

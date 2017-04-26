using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public static  class StyleController
    {
        public static FrameworkElement ApplicationRootElement = null;
        public static ResourceDictionary ResourceDictionary = null;


        public static void Initialize(FrameworkElement applicationRootElement, ResourceDictionary resourceDictionary)
        {
            ApplicationRootElement = applicationRootElement;
            ResourceDictionary = resourceDictionary;
        }


        public static object FindResource(string name)
        {
            if (ResourceDictionary == null)
                throw new Exception("ResourceDictionary has not been set.");

            if (ApplicationRootElement == null)
                throw new Exception("ApplicationRootElement has not been set.");


            if (ResourceDictionary.Contains(name)) return ResourceDictionary[name];

            return ApplicationRootElement.FindResource(name);
        }

        internal static object FindResource(this FrameworkElement root, string name)
        {
            if (root != null && root.Resources.Contains(name)) return root.Resources[name];

            try
            {
                return root.FindName(name);
            }
            catch { }
            return null;
        }

    }
}

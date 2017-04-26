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
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Reflection;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    
        // Converter to figure out how much the text needs to be moved.
        public class WidthToTranslate : IValueConverter
        {
            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                Double width = (Double)value;
                //return width / 4;
                return (10d);
            }

            public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
        // Converter to calculate the height of the rectangle that contains the header text.
        // This converter should be passed an item in the collection.
        public class RectangleHeight : IValueConverter
        {
            #region IValueConverter Members
            // Static to check if header height has been calculated.
            static double headerHeight = 0;

            public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                // If header height has not been calculated, go ahead and calculate,
                // otherwise just return the previously calculated value.
                if (headerHeight == 0)
                {
                    // Get the type of the item passed in.
                    Type valueType = value.GetType();

                    // Get the properties of the type.
                    PropertyInfo[] properties = valueType.GetProperties();

                    TextBlock tb = new TextBlock();
                    int propLength = 0;
                    // Get the longest property name and set the TextBlock text to the name.
                    foreach (PropertyInfo p in properties)
                        if (p.Name.Length > propLength)
                        {
                            tb.Text = p.Name;
                            propLength = p.Name.Length;
                        }

                    //Return the width of the textblock plus some padding.
                    headerHeight = tb.ActualWidth + 5;
                    return headerHeight;
                }
                // If the value has already been calulated, then return it.
                return headerHeight;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        } 
    
}

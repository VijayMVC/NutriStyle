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
using System.Xml.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class RadioButtonConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Contact.GenderOptions))
            {
                return false;
            }

            return ((Contact.GenderOptions)value).ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(Contact.GenderOptions), parameter.ToString(), true) : false;//null
        }
    }

    public class RadioButtonConverterMenuFood : IValueConverter
    {

        public object Convert(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Engine.FormData.Food.MenuFoodOptions))
            {
                return false;
            }

            return ((Engine.FormData.Food.MenuFoodOptions)value).ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(Engine.FormData.Food.MenuFoodOptions), parameter.ToString(), true) : false;//null
        }
    }

    public class RadioButtonConverterAvailableToAllUsers : IValueConverter
    {

        public object Convert(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Engine.FormData.Food.AvailableToAllUsersOptions))
            {
                return false;
            }

            return ((Engine.FormData.Food.AvailableToAllUsersOptions)value).ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(Engine.FormData.Food.AvailableToAllUsersOptions), parameter.ToString(), true) : false;//null
        }
    }
    public class RadioButtonConverterRecipeAvailableToAllUsers : IValueConverter
    {

        public object Convert(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Engine.FormData.Recipe.AvailableToAllUsersOptions))
            {
                return false;
            }

            return ((Engine.FormData.Recipe.AvailableToAllUsersOptions)value).ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(Engine.FormData.Recipe.AvailableToAllUsersOptions), parameter.ToString(), true) : false;//null
        }
    }
}






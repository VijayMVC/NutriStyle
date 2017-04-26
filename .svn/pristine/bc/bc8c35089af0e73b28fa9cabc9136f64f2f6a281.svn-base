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
using System.ComponentModel;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class BarChartData : INotifyPropertyChanged
    {

        private string name;
        private decimal value;

            public String Name {
                get { return (name); }
                set
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
            public decimal Value {
                get { return (value); }
                set
                {
                    this.value = value;
                    RaisePropertyChanged("Value");

                }
            }

            public BarChartData() { }
            public BarChartData(String name, decimal value) {

                this.name = name;
                this.value = value;
            }


            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            void RaisePropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion
    }
}

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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.ComponentModel;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.FormData
{
    public class ActivityLog :  INotifyPropertyChanged
    {

        private List<BarChartData> listBurned;
        private List<BarChartData> listConsumed;
        private List<BarChartData> listDifference;
        private String lossGain;

        private decimal weeklyTotal;
        private decimal weeklyWeightLoss;

        public String LossGain
        {
            get { return (lossGain); }

            set
            {
                lossGain = value;
                OnPropertyChanged("LossGain");
            }
        }

        public ActivityLog()
        {
            listBurned       = new List<BarChartData>();
            listConsumed     = new List<BarChartData>();
            listDifference   = new List<BarChartData>();

            weeklyTotal      = 0m;
            weeklyWeightLoss = 0m;
            lossGain = "loss";
        }
        public List<BarChartData> Burned {
            get { return (listBurned); }
            set
            {
                listBurned = value;
            }
        }

        public List<BarChartData> Consumed
        {
            get { return (listConsumed); }
            set
            {
                listConsumed = value;
            }
        }

        public List<BarChartData> Difference
        {
            get { return (listDifference); }
            set
            {
                listDifference = value;
            }
        }
        public void SetBurned(String name, decimal value)
        {
            BarChartData burned = null;
            BarChartData consumed = null;
            
            foreach (BarChartData bcd in listBurned)
            {
                if (bcd.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Value = value;
                    burned = bcd;
                    break;
                }
            }
            //find consumed
            foreach (BarChartData bcd in listConsumed)
            {
                if (bcd.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    consumed = bcd;
                    break;
                }
            }
            SetDifference(burned, consumed);
        }
        public void SetConsumed(String name, decimal value)
        {
            BarChartData burned = null;
            BarChartData consumed = null;

            foreach (BarChartData bcd in listConsumed)
            {
                if (bcd.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Value = value;
                    consumed = bcd;
                    break;
                }
            }
            //find burned
            foreach (BarChartData bcd in listBurned)
            {
                if (bcd.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    burned = bcd;
                    break;
                }
            }
            SetDifference(burned, consumed);
        }

        private void SetDifference(BarChartData burned, BarChartData consumed) {
            //set differences
            bool setDifference = false;
            if (burned != null && consumed != null)
            {
                foreach (BarChartData bcd in listDifference)
                {
                    if (bcd.Name.Equals(burned.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        bcd.Value = consumed.Value - burned.Value;
                        setDifference = true;
                        break;
                    }
                }
                if (!setDifference)
                {
                    listDifference.Add(new BarChartData(burned.Name, consumed.Value - burned.Value));
                }
            }
            CalculateWeekly();
        }

        public decimal WeeklyTotal
        {
            get {
                return (weeklyTotal); 
            }
            set
            {
                weeklyTotal = value;
                OnPropertyChanged("WeeklyTotal");
            }
        }
        /// <summary>
        /// Returns an absolute value
        /// </summary>
        public decimal WeeklyWeightLoss {
            get {
                
                return (Math.Abs(weeklyWeightLoss)); 
                //return (weeklyWeightLoss); 
            }

            set
            {
                weeklyWeightLoss = value;
                OnPropertyChanged("WeeklyWeightLoss");
            }
        }

        public void CalculateWeekly()
        {
            WeeklyTotal = 0;
            foreach (BarChartData bcd in listDifference)
            {
                WeeklyTotal += bcd.Value;
            }
            WeeklyWeightLoss = 0;
            if (WeeklyTotal != 0)
            {
                WeeklyWeightLoss = weeklyTotal / 3500m;
            }
            if (weeklyWeightLoss > 0)
            {
                LossGain = "gain";
            }
            else
            {
                LossGain = "loss";
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


}

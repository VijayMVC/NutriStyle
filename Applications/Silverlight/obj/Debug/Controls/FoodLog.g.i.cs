﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\Controls\FoodLog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "45290B02CFD81852B9FB626FCDF0B2ED"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace DynamicConnections.NutriStyle.MenuGenerator.Controls {
    
    
    public partial class FoodLog : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.StackPanel wrapper;
        
        internal System.Windows.Controls.TextBlock Label;
        
        internal System.Windows.Controls.Button Save;
        
        internal System.Windows.Controls.Button PrintDay;
        
        internal System.Windows.Controls.Button PrintEntireWeek;
        
        internal System.Windows.Controls.Button NutrientDetails;
        
        internal System.Windows.Controls.DataGrid dataGridBreakfast;
        
        internal System.Windows.Controls.DataGrid dataGridMorningSnack;
        
        internal System.Windows.Controls.DataGrid dataGridLunch;
        
        internal System.Windows.Controls.DataGrid dataGridAfternoonSnack;
        
        internal System.Windows.Controls.DataGrid dataGridDinner;
        
        internal System.Windows.Controls.DataGrid dataGridEveningSnack;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/Controls/FoodLog.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.wrapper = ((System.Windows.Controls.StackPanel)(this.FindName("wrapper")));
            this.Label = ((System.Windows.Controls.TextBlock)(this.FindName("Label")));
            this.Save = ((System.Windows.Controls.Button)(this.FindName("Save")));
            this.PrintDay = ((System.Windows.Controls.Button)(this.FindName("PrintDay")));
            this.PrintEntireWeek = ((System.Windows.Controls.Button)(this.FindName("PrintEntireWeek")));
            this.NutrientDetails = ((System.Windows.Controls.Button)(this.FindName("NutrientDetails")));
            this.dataGridBreakfast = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridBreakfast")));
            this.dataGridMorningSnack = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridMorningSnack")));
            this.dataGridLunch = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridLunch")));
            this.dataGridAfternoonSnack = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridAfternoonSnack")));
            this.dataGridDinner = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridDinner")));
            this.dataGridEveningSnack = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridEveningSnack")));
        }
    }
}


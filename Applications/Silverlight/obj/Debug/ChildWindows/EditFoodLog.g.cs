﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\ChildWindows\EditFoodLog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "69EB98A98BA3D63CF2D46C51BF829100"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
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
using System.Windows.Controls.DataVisualization.Charting;
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


namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows {
    
    
    public partial class EditFoodLog : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.ImageBrush backgroundImage;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.StackPanel wrapper;
        
        internal System.Windows.Controls.TextBlock Label;
        
        internal System.Windows.Controls.Button Save;
        
        internal System.Windows.Controls.DataGrid dataGridBreakfast;
        
        internal System.Windows.Controls.DataGrid dataGridLunch;
        
        internal System.Windows.Controls.DataGrid dataGridDinner;
        
        internal System.Windows.Controls.DataVisualization.Charting.Chart pieChart;
        
        internal System.Windows.Controls.DataVisualization.Charting.PieSeries pieSeries;
        
        internal System.Windows.Controls.Button OKButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/ChildWindows/EditFoodLog.x" +
                        "aml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.backgroundImage = ((System.Windows.Media.ImageBrush)(this.FindName("backgroundImage")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.wrapper = ((System.Windows.Controls.StackPanel)(this.FindName("wrapper")));
            this.Label = ((System.Windows.Controls.TextBlock)(this.FindName("Label")));
            this.Save = ((System.Windows.Controls.Button)(this.FindName("Save")));
            this.dataGridBreakfast = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridBreakfast")));
            this.dataGridLunch = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridLunch")));
            this.dataGridDinner = ((System.Windows.Controls.DataGrid)(this.FindName("dataGridDinner")));
            this.pieChart = ((System.Windows.Controls.DataVisualization.Charting.Chart)(this.FindName("pieChart")));
            this.pieSeries = ((System.Windows.Controls.DataVisualization.Charting.PieSeries)(this.FindName("pieSeries")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
        }
    }
}


﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\ChildWindows\FoodNutrients.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1DB220820CB83D5884A9DE5438D4C240"
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


namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows {
    
    
    public partial class FoodNutrients : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Style HorizontalLabelStyle;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.ImageBrush backgroundImage;
        
        internal System.Windows.Controls.TextBlock FoodName;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.DataGrid dataGrid;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/ChildWindows/FoodNutrients" +
                        ".xaml", System.UriKind.Relative));
            this.HorizontalLabelStyle = ((System.Windows.Style)(this.FindName("HorizontalLabelStyle")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.backgroundImage = ((System.Windows.Media.ImageBrush)(this.FindName("backgroundImage")));
            this.FoodName = ((System.Windows.Controls.TextBlock)(this.FindName("FoodName")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.dataGrid = ((System.Windows.Controls.DataGrid)(this.FindName("dataGrid")));
        }
    }
}


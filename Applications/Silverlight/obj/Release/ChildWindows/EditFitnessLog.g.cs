﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\ChildWindows\EditFitnessLog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0078633580309B1B5371F9A2938E4D90"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
    
    
    public partial class EditFitnessLog : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.ImageBrush backgroundImage;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.TextBlock Label;
        
        internal System.Windows.Controls.Button Save;
        
        internal System.Windows.Controls.DataGrid dataGrid1;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/ChildWindows/EditFitnessLo" +
                        "g.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.backgroundImage = ((System.Windows.Media.ImageBrush)(this.FindName("backgroundImage")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.Label = ((System.Windows.Controls.TextBlock)(this.FindName("Label")));
            this.Save = ((System.Windows.Controls.Button)(this.FindName("Save")));
            this.dataGrid1 = ((System.Windows.Controls.DataGrid)(this.FindName("dataGrid1")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
        }
    }
}


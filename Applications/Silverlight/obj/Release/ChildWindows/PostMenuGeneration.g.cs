﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\ChildWindows\PostMenuGeneration.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "75FB254F6CC8317BC98BA35F0262886F"
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
    
    
    public partial class PostMenuGeneration : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.ImageBrush backgroundImage;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.CheckBox shoppingList;
        
        internal System.Windows.Controls.CheckBox foodLog;
        
        internal System.Windows.Controls.Button OKButton;
        
        internal System.Windows.Controls.Button Close;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/ChildWindows/PostMenuGener" +
                        "ation.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.backgroundImage = ((System.Windows.Media.ImageBrush)(this.FindName("backgroundImage")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.shoppingList = ((System.Windows.Controls.CheckBox)(this.FindName("shoppingList")));
            this.foodLog = ((System.Windows.Controls.CheckBox)(this.FindName("foodLog")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
            this.Close = ((System.Windows.Controls.Button)(this.FindName("Close")));
        }
    }
}


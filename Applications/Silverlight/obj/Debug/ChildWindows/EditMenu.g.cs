﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\ChildWindows\EditMenu.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5F9CE3D122EDBE693D65B8D93849BD43"
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
    
    
    public partial class EditMenu : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.ImageBrush backgroundImage;
        
        internal System.Windows.Controls.TextBox name;
        
        internal System.Windows.Controls.TextBox description;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/ChildWindows/EditMenu.xaml" +
                        "", System.UriKind.Relative));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.backgroundImage = ((System.Windows.Media.ImageBrush)(this.FindName("backgroundImage")));
            this.name = ((System.Windows.Controls.TextBox)(this.FindName("name")));
            this.description = ((System.Windows.Controls.TextBox)(this.FindName("description")));
            this.OKButton = ((System.Windows.Controls.Button)(this.FindName("OKButton")));
        }
    }
}


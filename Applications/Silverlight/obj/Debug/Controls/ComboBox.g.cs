﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\Controls\ComboBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "726DD3D1EDC40E07493BB0538C358170"
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


namespace DynamicConnections.NutriStyle.MenuGenerator.Controls {
    
    
    public partial class ComboBox : System.Windows.Controls.UserControl {
        
        internal System.Windows.Style textBoxStyleCustomDisabled;
        
        internal System.Windows.Style ComboToggleButton;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ProgressBar progressBar;
        
        internal System.Windows.Controls.AutoCompleteBox MyAutoCompleteBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/Controls/ComboBox.xaml", System.UriKind.Relative));
            this.textBoxStyleCustomDisabled = ((System.Windows.Style)(this.FindName("textBoxStyleCustomDisabled")));
            this.ComboToggleButton = ((System.Windows.Style)(this.FindName("ComboToggleButton")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.progressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("progressBar")));
            this.MyAutoCompleteBox = ((System.Windows.Controls.AutoCompleteBox)(this.FindName("MyAutoCompleteBox")));
        }
    }
}


﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\Pages\FoodDislikes.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DED7168E581D5CAD37EEF4F276322177"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DynamicConnections.NutriStyle.MenuGenerator.Controls;
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


namespace DynamicConnections.NutriStyle.MenuGenerator.Pages {
    
    
    public partial class FoodDislikes : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label name;
        
        internal System.Windows.Controls.RadioButton foodGroupRadio;
        
        internal System.Windows.Controls.RadioButton foodUniqueRadio;
        
        internal System.Windows.Controls.Label SelectFoodLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation food;
        
        internal System.Windows.Controls.Button AddFavorite;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.DataGrid dataGrid1;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/Pages/FoodDislikes.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.name = ((System.Windows.Controls.Label)(this.FindName("name")));
            this.foodGroupRadio = ((System.Windows.Controls.RadioButton)(this.FindName("foodGroupRadio")));
            this.foodUniqueRadio = ((System.Windows.Controls.RadioButton)(this.FindName("foodUniqueRadio")));
            this.SelectFoodLabel = ((System.Windows.Controls.Label)(this.FindName("SelectFoodLabel")));
            this.food = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("food")));
            this.AddFavorite = ((System.Windows.Controls.Button)(this.FindName("AddFavorite")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.dataGrid1 = ((System.Windows.Controls.DataGrid)(this.FindName("dataGrid1")));
        }
    }
}

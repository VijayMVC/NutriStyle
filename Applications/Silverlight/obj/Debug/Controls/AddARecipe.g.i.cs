﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\Controls\AddARecipe.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3A7EF54621280F1B0F3A071A8B87B3E2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
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


namespace DynamicConnections.NutriStyle.MenuGenerator.Controls {
    
    
    public partial class AddARecipe : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.Grid FormData;
        
        internal System.Windows.Controls.ValidationSummary ValidationSummary;
        
        internal System.Windows.Controls.Grid Grid;
        
        internal System.Windows.Controls.TextBlock dc_nameLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_name;
        
        internal System.Windows.Controls.TextBlock dc_sourceofinformationdateobtainedLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_sourceofinformationdateobtained;
        
        internal System.Windows.Controls.TextBlock dc_preparationtimeLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_preparationtime;
        
        internal System.Windows.Controls.TextBlock dc_portion_amountLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_portion_amount;
        
        internal System.Windows.Controls.TextBlock dc_portiontypeidLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation dc_portiontypeid;
        
        internal System.Windows.Controls.TextBlock dc_numberofservingsLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_numberofservings;
        
        internal System.Windows.Controls.TextBlock dc_directionsLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox dc_directions;
        
        internal System.Windows.Controls.Button AddIngredient;
        
        internal System.Windows.Controls.DataGrid DataGrid;
        
        internal System.Windows.Controls.TextBlock dc_availabletoallusersLabel;
        
        internal System.Windows.Controls.RadioButton availableToAllUsersYes;
        
        internal System.Windows.Controls.RadioButton availableToAllUsersNo;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/Controls/AddARecipe.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.FormData = ((System.Windows.Controls.Grid)(this.FindName("FormData")));
            this.ValidationSummary = ((System.Windows.Controls.ValidationSummary)(this.FindName("ValidationSummary")));
            this.Grid = ((System.Windows.Controls.Grid)(this.FindName("Grid")));
            this.dc_nameLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_nameLabel")));
            this.dc_name = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_name")));
            this.dc_sourceofinformationdateobtainedLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_sourceofinformationdateobtainedLabel")));
            this.dc_sourceofinformationdateobtained = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_sourceofinformationdateobtained")));
            this.dc_preparationtimeLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_preparationtimeLabel")));
            this.dc_preparationtime = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_preparationtime")));
            this.dc_portion_amountLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_portion_amountLabel")));
            this.dc_portion_amount = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_portion_amount")));
            this.dc_portiontypeidLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_portiontypeidLabel")));
            this.dc_portiontypeid = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("dc_portiontypeid")));
            this.dc_numberofservingsLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_numberofservingsLabel")));
            this.dc_numberofservings = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_numberofservings")));
            this.dc_directionsLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_directionsLabel")));
            this.dc_directions = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.WatermarkTextBox)(this.FindName("dc_directions")));
            this.AddIngredient = ((System.Windows.Controls.Button)(this.FindName("AddIngredient")));
            this.DataGrid = ((System.Windows.Controls.DataGrid)(this.FindName("DataGrid")));
            this.dc_availabletoallusersLabel = ((System.Windows.Controls.TextBlock)(this.FindName("dc_availabletoallusersLabel")));
            this.availableToAllUsersYes = ((System.Windows.Controls.RadioButton)(this.FindName("availableToAllUsersYes")));
            this.availableToAllUsersNo = ((System.Windows.Controls.RadioButton)(this.FindName("availableToAllUsersNo")));
        }
    }
}


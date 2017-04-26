﻿#pragma checksum "C:\Builds\NutriStyle\Applications\Silverlight\Controls\Profile.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1669D7390A0FD8E2AE07CAED99810494"
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
    
    
    public partial class Profile : System.Windows.Controls.UserControl {
        
        internal System.Windows.Style radioButtonWithImage;
        
        internal System.Windows.Controls.BusyIndicator busyIndicator;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Label name;
        
        internal System.Windows.Controls.Button Next;
        
        internal System.Windows.Controls.ValidationSummary ValidationSummary;
        
        internal System.Windows.Controls.StackPanel Name;
        
        internal System.Windows.Controls.TextBox FirstName;
        
        internal System.Windows.Controls.TextBox LastName;
        
        internal System.Windows.Controls.CheckBox RollParentShoppingList;
        
        internal System.Windows.Controls.RadioButton femaleButton;
        
        internal System.Windows.Controls.Image femaleImage;
        
        internal System.Windows.Controls.RadioButton maleButton;
        
        internal System.Windows.Controls.Image maleImage;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation BirthMonth;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation BirthDay;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation BirthYear;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation HeightFeet;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation HeightInches;
        
        internal System.Windows.Controls.TextBox CurrentWeight;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation ActivityLevel;
        
        internal System.Windows.Controls.CheckBox maintainWeight;
        
        internal System.Windows.Controls.Label maintainWeightLabel;
        
        internal System.Windows.Controls.StackPanel targetWeightLabel;
        
        internal System.Windows.Controls.TextBox targetWeight;
        
        internal System.Windows.Controls.StackPanel poundsPerWeekLabel;
        
        internal DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation PoundsPerWeek;
        
        internal System.Windows.Controls.StackPanel iWantToLose;
        
        internal System.Windows.Controls.StackPanel BottomNext;
        
        internal System.Windows.Controls.Button EditAdditionalProfiles;
        
        internal System.Windows.Controls.StackPanel BottomSave;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/Controls/Profile.xaml", System.UriKind.Relative));
            this.radioButtonWithImage = ((System.Windows.Style)(this.FindName("radioButtonWithImage")));
            this.busyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("busyIndicator")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.name = ((System.Windows.Controls.Label)(this.FindName("name")));
            this.Next = ((System.Windows.Controls.Button)(this.FindName("Next")));
            this.ValidationSummary = ((System.Windows.Controls.ValidationSummary)(this.FindName("ValidationSummary")));
            this.Name = ((System.Windows.Controls.StackPanel)(this.FindName("Name")));
            this.FirstName = ((System.Windows.Controls.TextBox)(this.FindName("FirstName")));
            this.LastName = ((System.Windows.Controls.TextBox)(this.FindName("LastName")));
            this.RollParentShoppingList = ((System.Windows.Controls.CheckBox)(this.FindName("RollParentShoppingList")));
            this.femaleButton = ((System.Windows.Controls.RadioButton)(this.FindName("femaleButton")));
            this.femaleImage = ((System.Windows.Controls.Image)(this.FindName("femaleImage")));
            this.maleButton = ((System.Windows.Controls.RadioButton)(this.FindName("maleButton")));
            this.maleImage = ((System.Windows.Controls.Image)(this.FindName("maleImage")));
            this.BirthMonth = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("BirthMonth")));
            this.BirthDay = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("BirthDay")));
            this.BirthYear = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("BirthYear")));
            this.HeightFeet = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("HeightFeet")));
            this.HeightInches = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("HeightInches")));
            this.CurrentWeight = ((System.Windows.Controls.TextBox)(this.FindName("CurrentWeight")));
            this.ActivityLevel = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("ActivityLevel")));
            this.maintainWeight = ((System.Windows.Controls.CheckBox)(this.FindName("maintainWeight")));
            this.maintainWeightLabel = ((System.Windows.Controls.Label)(this.FindName("maintainWeightLabel")));
            this.targetWeightLabel = ((System.Windows.Controls.StackPanel)(this.FindName("targetWeightLabel")));
            this.targetWeight = ((System.Windows.Controls.TextBox)(this.FindName("targetWeight")));
            this.poundsPerWeekLabel = ((System.Windows.Controls.StackPanel)(this.FindName("poundsPerWeekLabel")));
            this.PoundsPerWeek = ((DynamicConnections.NutriStyle.MenuGenerator.Controls.ComboBoxWithValidation)(this.FindName("PoundsPerWeek")));
            this.iWantToLose = ((System.Windows.Controls.StackPanel)(this.FindName("iWantToLose")));
            this.BottomNext = ((System.Windows.Controls.StackPanel)(this.FindName("BottomNext")));
            this.EditAdditionalProfiles = ((System.Windows.Controls.Button)(this.FindName("EditAdditionalProfiles")));
            this.BottomSave = ((System.Windows.Controls.StackPanel)(this.FindName("BottomSave")));
        }
    }
}


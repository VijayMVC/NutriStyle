﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using System.Xml.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.DataVisualization.Charting;
using System.Text;
using System.Windows.Markup;
using DynamicConnections.NutriStyle.MenuGenerator.Engine;
using System.Windows.Printing;
using System.Windows.Media.Imaging;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class EditShoppingList : Page
    {
        
        private double spacetoPrint { get; set; }
        private double CanvasTop { get; set; }

        private Canvas PrintBody { get; set; }


        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public EditShoppingList()
        {
            InitializeComponent();
            //Setup birthday values
            shoppingList.ContactId = new Guid(General.ContactId());
        }
    }
}

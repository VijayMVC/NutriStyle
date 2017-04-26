using System;
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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using DynamicConnections.NutriStyle.MenuGenerator.Engine;
using System.Xml.Linq;
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Data;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class MenuEditor : ChildWindow
    {
        Guid menuId = Guid.Empty;

        public MenuEditor(Guid menuId)
        {
            InitializeComponent();
            this.menuId = menuId;
            PopulateHelp hp = new PopulateHelp();

            macroChart.PopulateChartDailyMenu(menuId);
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}


using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class Food
    {

        public decimal PortionSize { get; set; }
        public decimal Fat { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbohydrate { get; set; }
        public decimal Alcohol { get; set; }
        public decimal UnitGramWeight { get; set; }
        public PairWithList PortionType { get; set; }
        public bool IsRecipe { get; set; }
        public String PortionTypeName { get; set; }
        public String PortionTypeAbbreviation { get; set; }

    }
}

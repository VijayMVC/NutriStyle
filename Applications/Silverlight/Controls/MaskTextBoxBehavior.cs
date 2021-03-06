﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public class MaskTextBoxBehavior : Behavior<WatermarkTextBox>
    {
        public static readonly DependencyProperty TrueTextProperty =
            DependencyProperty.Register("TrueText", typeof(string), typeof(MaskTextBoxBehavior), null);

        public string TrueText
        {
            get
            {
                return (string)GetValue(TrueTextProperty);
            }

            set
            {
                SetValue(TrueTextProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TextChanged += new TextChangedEventHandler(AssociatedObject_TextChanged);
            TrueText = string.Empty;
        }

        void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            int index = AssociatedObject.SelectionStart;
            if (AssociatedObject.Text.Length > 0)
            {
                String newString = String.Empty;
                if (index > 0)
                {
                    newString = AssociatedObject.Text.Substring(index - 1, 1);
                }

                if (TrueText.Length > AssociatedObject.Text.Length)
                {
                    TrueText = TrueText.Remove(index, 1);
                }
                else if (TrueText.Length != AssociatedObject.Text.Length)
                {
                    TrueText = TrueText.Insert(index - 1, newString);
                }
                AssociatedObject.TrueText = TrueText;
                AssociatedObject.Text = new string('*', AssociatedObject.Text.Length);
                AssociatedObject.SelectionStart = index;
            }
            else if (AssociatedObject.Text.Length == 0)
            {
                TrueText = String.Empty;
            }
        }
    }
}

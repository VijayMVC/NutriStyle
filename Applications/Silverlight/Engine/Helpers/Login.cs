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
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class Login : INotifyPropertyChanged
    {
        private String verificationCode = String.Empty;
        private String zipcode = String.Empty;
        private String email = String.Empty;
        private String password = String.Empty;
        private String passwordVerify = String.Empty;


        public String FirstName { get; set; }
        public String LastName { get; set; }

        public PairWithList Country { get; set; }
        public PairWithList GrocerPrimary { get; set; }
        public PairWithList GrocerSecondary { get; set; }
        public PairWithList GrocerTertiary { get; set; }
        public String GrocerOther { get; set; }

        public String Password
        {
            get
            {
                
                return (password);
            }
            
            set
            {
                if (String.IsNullOrEmpty(value) || value.Equals("password", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter password");
                }
               
                    password = value;
                    OnPropertyChanged("Password");
                
            }
        }
        public String PasswordVerify
        {
            get
            {
                return (passwordVerify);
            }

            set
            {
                if (!password.Equals(value) && password.Length > 0)
                {
                    throw new Exception("Passwords do not match.  Please re-enter");
                }
                else if (String.IsNullOrEmpty(value) || value.Equals("password", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter verification password");
                }
                else
                {
                    passwordVerify = value;
                    OnPropertyChanged("PasswordVerify");
                }
            }
        }

        public String Email
        {
            get
            {
                return (email);
            }

            set
            {
                if (value.Equals("Email AddresS", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter email address");
                }
                /*
                if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"))*/
                //\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*

                if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                {
                    throw new Exception("Email is not in the correct format");
                }
                else
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public String ZipCode
        {
            get
            {
                return (zipcode);
            }

            set
            {
                if (String.IsNullOrEmpty(value) || value.Equals("postal code", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter zipcode");
                }
                else
                {
                    zipcode = value;
                    OnPropertyChanged("ZipCode");
                }
            }
        }

        public String VerificationCode {
            get
            {
                return (verificationCode);
            }

            set
            {
                if (String.IsNullOrEmpty(value) || value.Equals("verification code", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter verification code");
                }
                else
                {
                    verificationCode = value;
                    OnPropertyChanged("VerificationCode");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
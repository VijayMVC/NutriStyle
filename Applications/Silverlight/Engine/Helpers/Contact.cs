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
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class Contact : INotifyPropertyChanged
    {
        public Contact()
        {
            userGender = GenderOptions.None;
        }

        private int weight;
        private int? targetWeight;
        private PairWithList heightFeet;
        private PairWithList heightInches;
        private PairWithList birthDay;
        private PairWithList birthMonth;
        private PairWithList birthYear;
        private PairWithList activityLevel;
        private DateTime birthdate;
        private GenderOptions userGender;
        private PairWithList poundsPerWeek;
        private decimal? kcalTarget;

        private String firstname;
        private String lastname;

        public enum GenderOptions
        {
            None = 0,
            Male = 1,
            Female = 2
        }

        public String Email { get; set; }

        public String FirstName
        {
            get { return (firstname); }
            set
            {
                firstname = value;
                OnPropertyChanged("FirstName");
            }
        }


        public String LastName
        {
            get { return (lastname); }
            set
            {
                lastname = value;
                OnPropertyChanged("LastName");
            }
        }
        
        public DateTime Birthdate
        {
            get { return (birthdate); }
            set
            {
                var dtf = CultureInfo.CurrentCulture.DateTimeFormat;

                BirthYear = new PairWithList(value.Year.ToString(), value.Year.ToString(), String.Empty, new List<PairWithList>());
                BirthMonth = new PairWithList(dtf.GetAbbreviatedMonthName(value.Month), value.Month.ToString(), String.Empty, new List<PairWithList>());
                BirthDay = new PairWithList(value.Day.ToString(), value.Day.ToString(), String.Empty, new List<PairWithList>());
                this.birthdate = value;
            }
        }
        public decimal KcalCalculatedTarget { get; set; }

        public decimal WeightKG { get; set; }
        public decimal HeightM { get; set; }
        public decimal HeightCM { get; set; }

        public int Age { get; set; }
        public Pair Gender { get; set; }
        public decimal BMI { get; set; }
        public Guid MenuId { get; set; }

        public bool RollParentShoppingList { get; set; }

        public PairWithList BirthYear
        {
            get
            {
                return (birthYear);
            }
            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid birth year");
                }
                else
                {
                    birthYear = value;
                    OnPropertyChanged("BirthYear");
                }
            }
        }
        public PairWithList BirthMonth
        {
            get
            {
                return (birthMonth);
            }
            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid birth month");
                }
                else
                {
                    birthMonth = value;
                    OnPropertyChanged("BirthMonth");
                }
            }
        }
        public PairWithList BirthDay
        {
            get
            {
                return (birthDay);
            }
            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid birth day");
                }
                else
                {
                    birthDay = value;
                    OnPropertyChanged("BirthDay");
                }
            }
        }

        public PairWithList HeightInches
        {
            get
            {
                return (heightInches);
            }

            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid height");
                }
                else
                {
                    heightInches = value;
                    OnPropertyChanged("HeightInches");
                }
            }
        }
        public PairWithList HeightFeet
        {
            get
            {
                return (heightFeet);
            }
            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid height");
                }
                else
                {
                    heightFeet = value;
                    OnPropertyChanged("HeightFeet");
                }
            }
        }

        public int CurrentWeight
        {
            get
            {
                return (weight);
            }
            set
            {
                if (value < 1)
                {
                    throw new Exception("Please enter valid weight");
                }
                else
                {
                    weight = value;
                    OnPropertyChanged("CurrentWeight");
                }
            }
        }

        public PairWithList ActivityLevel
        {
            get
            {
                return (activityLevel);
            }
            set
            {
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter valid activity level");
                }
                else
                {
                    activityLevel = value;
                    OnPropertyChanged("ActivityLevel");
                }
            }
        }

        public bool MaintainTargetWeight { get; set; }
        public int? TargetWeight
        {
            get
            {
                return (targetWeight);
            }
            set
            {
                if (value == int.MinValue)
                {
                    value = null;
                    OnPropertyChanged("TargetWeight");
                }
                else if (!MaintainTargetWeight && (value == null ||
                    (value < 0))
                )
                {
                    throw new Exception("Please enter valid target weight");
                }
                else
                {
                    targetWeight = value;
                    OnPropertyChanged("TargetWeight");
                }
            }
        }
        public PairWithList PoundsPerWeek
        {
            get
            {
                return (poundsPerWeek);
            }
            set
            {
                if (!MaintainTargetWeight && (String.IsNullOrEmpty(((PairWithList)value).Name) && String.IsNullOrEmpty(((PairWithList)value).Id))
                )
                {
                    throw new Exception("Please enter valid Pounds Per Week selection");
                }
                else
                {
                    poundsPerWeek = value;
                    OnPropertyChanged("PoundsPerWeek");
                }
            }
        }

        public Guid ContactId { get; set; }

        public bool MorningSnack { get; set; }
        public bool AfternoonSnack { get; set; }
        public bool EveningSnack { get; set; }

        public Guid PresetId { get; set; }

        public decimal DEE { get; set; }
        public decimal? KcalTarget
        {
            get
            {
                return (kcalTarget);
            }
            set
            {
                if (UserSpecifiedKcalTarget)
                {
                    if (value == decimal.MinValue)
                    {
                        throw new Exception("Please enter valid calorie target");
                    }
                    else if (value < 1400)
                    {
                        throw new Exception("Please enter valid calorie target");
                    }
                    else
                    {
                        kcalTarget = value;
                        OnPropertyChanged("KcalTarget");
                    }
                }
                else
                {
                    if (value == decimal.MinValue)
                    {
                        kcalTarget = null;
                    }
                    else
                    {
                        kcalTarget = value;
                    }
                    OnPropertyChanged("KcalTarget");
                }
            }
        }
        public bool UserSpecifiedKcalTarget { get; set; }

        public String MaleImage
        {
            get
            {
                if (Gender != null && Gender.Id.Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    return ("/DynamicConnections.NutriStyle.MenuGenerator;component/Images/male_on.png");
                }
                else//must be female
                {
                    return ("/DynamicConnections.NutriStyle.MenuGenerator;component/Images/male_off.png");
                }
            }
        }
        public String FemaleImage
        {
            get
            {
                if (Gender != null && Gender.Id.Equals("2", StringComparison.OrdinalIgnoreCase))
                {
                    return ("/DynamicConnections.NutriStyle.MenuGenerator;component/Images/female_on.png");
                }
                else//must be female
                {
                    return ("/DynamicConnections.NutriStyle.MenuGenerator;component/Images/female_off.png");
                }
            }
        }
        public bool IsMale
        {
            get
            {
                if (Gender != null && Gender.Id.Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            set
            {
                if (value)
                {
                    Gender = new Pair("Male", "1");
                }
            }
        }
        public bool IsFemale
        {
            get
            {
                if (Gender != null && Gender.Id.Equals("2", StringComparison.OrdinalIgnoreCase))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            set
            {
                if (value)
                {
                    Gender = new Pair("Female", "2");
                }
            }
        }

        public GenderOptions UserGender
        {
            get
            {
                if (Gender != null && Gender.Id.Equals("1"))
                {
                    userGender = GenderOptions.Male;
                }
                else if (Gender != null && Gender.Id.Equals("2"))
                {
                    userGender = GenderOptions.Female;
                }
                else
                {
                    userGender = GenderOptions.None;
                }
                return (userGender);
            }
            set
            {
                if (value.Equals(GenderOptions.None))
                {
                    throw new Exception("Please choose one");
                }

                userGender = value;
                if (userGender == GenderOptions.Male)
                {
                    Gender = new Pair("Male", "1");
                }
                else if (userGender == GenderOptions.Female)
                {
                    Gender = new Pair("Female", "2");
                }

                OnPropertyChanged("UserGender");

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

        public static Contact BuildContact(XElement result2)
        {
            Contact c = new Contact();
            try
            {
                c.ContactId = new Guid(result2.Descendants("contactid").First().Value);
                c.Email = RetrieveValue(result2, "emailaddress1") == null ? String.Empty : RetrieveValue(result2, "emailaddress1");
                c.Age = RetrieveValue(result2, "dc_age") == null ? 0 : Convert.ToInt32(RetrieveValue(result2, "dc_age"));
                c.BMI = RetrieveValue(result2, "dc_bmi") == null ? 0m : Convert.ToDecimal(RetrieveValue(result2, "dc_bmi"));
                c.MaintainTargetWeight = RetrieveValue(result2, "dc_maintaintargetweight") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_maintaintargetweight"));
                c.PoundsPerWeek = RetrievePairWithList(result2, "dc_poundsperweek", "Id") == null ? null : RetrievePairWithList(result2, "dc_poundsperweek", "Id");
                c.WeightKG = RetrieveValue(result2, "dc_weightkg") == null ? 0 : Convert.ToDecimal(RetrieveValue(result2, "dc_weightkg"));
                c.Birthdate = RetrieveValue(result2, "birthdate") == null ? DateTime.MinValue : Convert.ToDateTime(RetrieveValue(result2, "birthdate"));

                c.TargetWeight = RetrieveValue(result2, "dc_targetweight") == null ? null : (int?)Convert.ToInt32(RetrieveValue(result2, "dc_targetweight"));
                c.KcalCalculatedTarget = RetrieveValue(result2, "dc_kcalcalculatedtarget") == null ? 0 : Convert.ToDecimal(RetrieveValue(result2, "dc_kcalcalculatedtarget"));
                c.ActivityLevel = RetrievePairWithList(result2, "dc_activitylevel", "Id") == null ? null : RetrievePairWithList(result2, "dc_activitylevel", "Id");
                c.HeightCM = RetrieveValue(result2, "dc_heightcm") == null ? 0m : Convert.ToDecimal(RetrieveValue(result2, "dc_heightcm"));
                c.HeightM = c.HeightCM / 100m;
                c.HeightInches = RetrievePairWithList(result2, "dc_heightinches", "Id") == null ? null : RetrievePairWithList(result2, "dc_heightinches", "Id");
                c.Gender = RetrievePair(result2, "gendercode", "Id") == null ? null : RetrievePair(result2, "gendercode", "Id");
                c.HeightFeet = RetrievePairWithList(result2, "dc_heightfeet", "Id") == null ? null : RetrievePairWithList(result2, "dc_heightfeet", "Id");

                c.FirstName = RetrieveValue(result2, "firstname") == null ? String.Empty : RetrieveValue(result2, "firstname");
                c.LastName = RetrieveValue(result2, "lastname") == null ? String.Empty : RetrieveValue(result2, "lastname");



                c.MorningSnack              = RetrieveValue(result2, "dc_morningsnack") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_morningsnack"));
                c.AfternoonSnack            = RetrieveValue(result2, "dc_afternoonsnack") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_afternoonsnack"));
                c.EveningSnack              = RetrieveValue(result2, "dc_eveningsnack") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_eveningsnack"));

                c.PresetId                  = RetrieveAttribute(result2, "dc_menupresetid", "Id") == null ? Guid.Empty : new Guid(RetrieveAttribute(result2, "dc_menupresetid", "Id"));
                c.DEE                       = RetrieveValue(result2, "dc_dee") == null ? 0m : Convert.ToDecimal(RetrieveValue(result2, "dc_dee"));
                c.KcalTarget                = RetrieveValue(result2, "dc_kcaltarget") == null ? 0m : Convert.ToDecimal(RetrieveValue(result2, "dc_kcaltarget"));

                c.UserSpecifiedKcalTarget   = RetrieveValue(result2, "dc_userspecifiedkcaltarget") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_userspecifiedkcaltarget"));

                c.MenuId                    = RetrieveValue(result2, "dc_menu.dc_menuid") == null ? Guid.Empty : new Guid((RetrieveValue(result2, "dc_menu.dc_menuid")));

                c.CurrentWeight             = RetrieveValue(result2, "dc_currentweight") == null ? 0 : Convert.ToInt32(RetrieveValue(result2, "dc_currentweight"));
                c.RollParentShoppingList    = RetrieveValue(result2, "dc_rollshoppinglisttoparent") == null ? false : Convert.ToBoolean(RetrieveValue(result2, "dc_rollshoppinglisttoparent"));

            }
            catch (Exception)
            {
                //need to find a better solution.  Error is getting thrown when Properties that don't have values are populated.  TODO:  Find a better solution
            }
            return (c);
        }

        private static Pair RetrievePair(XElement node, String name, String attributeName)
        {
            if (node.Descendants(name) == null || node.Descendants(name).Count() == 0)
            {
                return (null);
            }
            return (new Pair()
            {
                Name = node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Value,
                Id = node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Attributes("Id").First().Value
            });
        }
        private static PairWithList RetrievePairWithList(XElement node, String name, String attributeName)
        {
            if (node.Descendants(name) == null || node.Descendants(name).Count() == 0)
            {
                return (null);
            }
            return (new PairWithList()
            {
                Name = node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Value,
                Id = node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Attributes("Id").First().Value,
                EntityType = String.Empty,
                List = new List<PairWithList>()
            });
        }

        private static String RetrieveValue(XElement node, String name)
        {
            if (node.Descendants(name) == null || node.Descendants(name).Count() == 0)
            {
                return (null);
            }
            return (node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Value);
        }
        private static String RetrieveAttribute(XElement node, String name, String attributeName)
        {
            if (node.Descendants(name) == null || node.Descendants(name).Count() == 0)
            {
                return (null);
            }
            return (node.Descendants(name).First().Value == null ? null : node.Descendants(name).First().Attributes("Id").First().Value);
        }

    }
}

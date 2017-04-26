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
using System.Collections.Generic;
using System.ComponentModel;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class ItemType
    {
        public String Name       { get; set; }
        public String Id         { get; set; }
    }

    public class Pair : INotifyPropertyChanged, IComparable
    {
        private String name;
        private String id;
        private String entityType;

        private static Pair _This;

        public static Pair This { get { return _This; } }

        public String Name
        {
            get { return (name); }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        public String Id
        {
            get { return (id); }
            set
            {
                id = value;
                RaisePropertyChanged("Id");
            }
        }
        public String EntityType
        {
            get { return (entityType); }
            set
            {
                entityType = value;
                RaisePropertyChanged("EntityType");
            }
        }
        public Pair() { _This = this; }

        public Pair(String name, String id)
        {
            this.name = name;
            this.id = id;
            _This = this;
        }
        public Pair(String name, String id, String entityType)
        {
            this.name = name;
            this.id = id;
            this.entityType = entityType;
            _This = this;
        }
        public override string ToString()
        {
            return (Name);
        }
        
        #region INotifyPropertyChanged
        
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Pair)
            {
                Pair p2 = (Pair)obj;
                return name.CompareTo(p2.Name);
            }
            else
            {
                MessageBox.Show("type: "+obj.GetType()+":"+name);
                throw new ArgumentException("Object is not a Pair.");
            }
        }

        #endregion
    }
    public class TypeComboBoxData : List<Pair>
    {
        public TypeComboBoxData()
        {
        }
    }


    public static class IDictionaryExtensions
    {
        public static TKey FindKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (value.Equals(pair.Value)) return pair.Key;

            throw new Exception("the value is not found in the dictionary");
        }
    }
}





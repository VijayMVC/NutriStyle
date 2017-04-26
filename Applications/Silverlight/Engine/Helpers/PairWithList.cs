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
  

    public class PairWithList : INotifyPropertyChanged, IComparable
    {
        private String name;
        private String id;
        private String entityType;
        private List<PairWithList> list;

        private static PairWithList _This;

        public static PairWithList This { get { return _This; } }

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
        public List<PairWithList> List
        {
            get { return (list); }
            set
            {
                list = value;
                RaisePropertyChanged("List");
            }
        }
        public PairWithList() { _This = this; }

        public PairWithList(String name, String id)
        {
            this.name = name;
            this.id = id;
            _This = this;
        }
        public PairWithList(String name, String id, String entityType, List<PairWithList> list)
        {
            this.name = name;
            this.id = id;
            this.entityType = entityType;
            this.list = list;

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
            if (obj is PairWithList)
            {
                PairWithList p2 = (PairWithList)obj;
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




}





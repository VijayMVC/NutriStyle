using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class Row : INotifyPropertyChanged
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        private Dictionary<string, string> _types = new Dictionary<string, string>();
        /// <summary>
        /// Lets the dev know that the value in the underlying record has changed and is not in sync with the database
        /// </summary>
        public bool RowChanged {get; set;}
        public bool CreateRow { get; set; }
        /// <summary>
        /// Gets the column names
        /// </summary>
        public ICollection<string> ColumnNames
        {
            get
            {
                return _data.Keys;
            }
        }
        public Dictionary<String, String> ColumnTypes
        {
            get
            {
                return _types;
            }
        }
        /// <summary>
        /// a string indexer for accessing the rows proeprties
        /// </summary>
        public object this[string index]
        {
            get
            {
                if (_data.ContainsKey(index))
                {
                    return _data[index];
                }
                else
                {
                    return (null);
                }
            }
            set
            {
                _data[index] = value;
                // any property changes need to be signalled to UI elements bound to the Data property
                OnPropertyChanged("Data");
            }
        }

        /// <summary>
        /// A property which is used for integrating with the binding framework.
        /// </summary>
        public object Data
        {
            get
            {
                // when the binding framework reads this property, simple return the Row instance. The
                // RowIndexConverter takes care of extracting the correct property value
                return this;
            }
            set
            {
                // the RowIndexConverter will signal property changes by providing an instance of PropertyValueChange.
                var setter = value as PropertyValueChange;
                _data[setter.PropertyName] = setter.Value;
                
                //RowChanged = true;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}






using System;

using System.ComponentModel;


namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class FeedBack : INotifyPropertyChanged
    {
        private String title = String.Empty;
        private String description = String.Empty;
        

        public String Title
        {
            get
            {
                
                return (title);
            }
            
            set
            {
                if (String.IsNullOrEmpty(value) || value.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter title");
                }
                else
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }
       
        public String Description
        {
            get
            {
                return (description);
            }

            set
            {
                if (String.IsNullOrEmpty(value) || value.Equals("description", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter description");
                }
                else
                {
                    description = value;
                    OnPropertyChanged("Description");
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

using System.ComponentModel;
using Codify.Extensions;

namespace Codify.Entities
{
    public abstract class NotifyableEntity : INotifyPropertyChanged
    {
        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Protected Methods

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler.ExecuteIfNotNull(this, e);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        protected bool SetValue<T>(ref T property, T value, string propertyName)
        {
            if (Equals(property, value))
            {
                return false;
            }
            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool SetValue<T>(ref T property, T value, params string[] propertyNames)
        {
            if (Equals(property, value))
            {
                return false;
            }
            property = value;
            RaisePropertyChanged(propertyNames);
            return true;
        }

        #endregion
    }
}
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

        protected void RaisePropertyChange(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChange(propertyName);
            }
        }

        #endregion
    }
}

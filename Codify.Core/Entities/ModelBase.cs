namespace Codify.Entities
{
    public abstract class ModelBase : NotifyableEntity
    {
        #region Protected Methods

        protected bool SetProperty<T>(ref T property, T value, string propertyName)
        {
            if (Equals(property, value))
            {
                return false;
            }
            property = value;
            RaisePropertyChange(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T property, T value, params string[] propertyNames)
        {
            if (Equals(property, value))
            {
                return false;
            }
            property = value;
            RaisePropertyChange(propertyNames);
            return true;
        }

        #endregion
    }
}
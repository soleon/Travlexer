using System;
using System.Windows;
using System.Windows.Controls;
using Codify;
using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (PlaceDetailsViewModel))]
    public partial class PlaceDetailsView
    {
        private PlaceDetailsViewModel _dataContext;

        public PlaceDetailsView()
        {
            InitializeComponent();
        }

        private void SelectAllOnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                throw new ArgumentException("Sender must be a TextBox.", "sender");
            }
            textBox.SelectAll();
        }

        protected override void OnDataContextChanged(object oldValue, object newValue)
        {
            var old = oldValue as PlaceDetailsViewModel;
            if (old != null)
            {
                old.DataStateChanged -= OnDataStateChanged;
            }
            _dataContext = newValue as PlaceDetailsViewModel;
            if (_dataContext != null)
            {
                _dataContext.DataStateChanged += OnDataStateChanged;
                OnDataStateChanged(_dataContext.Data.DataState);
            }
            base.OnDataContextChanged(oldValue, newValue);
        }

        private void OnDataStateChanged(DataStates state)
        {
            switch (state)
            {
                case DataStates.Busy:
                    VisualStateManager.GoToState(this, "Busy", false);
                    break;
                case DataStates.Error:
                    VisualStateManager.GoToState(this, "Error", false);
                    break;
                default:
                    VisualStateManager.GoToState(this, "None", false);
                    break;
            }
        }
    }
}
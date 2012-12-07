using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Codify;
using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (PlaceDetailsViewModel))]
    public partial class PlaceDetailsView
    {
        public PlaceDetailsView()
        {
            InitializeComponent();
            SetBinding(DataStateProperty, new Binding("Data.DataState"));
        }

        private void SelectAllOnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => ((TextBox) sender).SelectAll());
        }


        #region DataState

        public DataStates DataState
        {
            get { return (DataStates) GetValue(DataStateProperty); }
            set { SetValue(DataStateProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="DataState" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataStateProperty = DependencyProperty.Register(
            "DataState",
            typeof (DataStates),
            typeof (PlaceDetailsView),
            new PropertyMetadata(default(DataStates), (s, e) => ((PlaceDetailsView) s).OnDataStateChanged((DataStates) e.NewValue)));

        /// <summary>
        ///     Called when <see cref="DataState" /> changes.
        /// </summary>
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

        #endregion
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using Codify;
using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof(RouteDetailsViewModel))]
    public partial class RouteDetailsView
    {
        public RouteDetailsView()
        {
            InitializeComponent();
        }
    }
}
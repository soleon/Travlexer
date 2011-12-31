﻿using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Phone.Controls.Maps;
using Travelexer.WindowsPhone.Core.Collections;
using Travelexer.WindowsPhone.Core.Commands;
using Travelexer.WindowsPhone.Core.Extensions;
using Travelexer.WindowsPhone.Core.Services;
using Travelexer.WindowsPhone.Core.ViewModels;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Views;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Defines the view model for <see cref="MapView"/>
	/// </summary>
	public class MapViewModel : ViewModelBase
	{
		#region Private Fields

		private static readonly IDataContext _data = Globals.DataContext;

		#endregion


		#region Constructors

		public MapViewModel()
		{
			Pushpins = new AdaptedObservableCollection<Place, PushpinViewModel>(p => new PushpinViewModel(p, parent: this), _data.Places);
			Pushpins.CollectionChanged += OnPushpinsCollectionChanged;

			CommandAddPlace = new DelegateCommand<Location>(OnAddPlace);
			CommandToggleSelectedPushpin = new DelegateCommand<PushpinViewModel>(OnToggleSelectedPushpin);
			CommandDeselectPushpin = new DelegateCommand<PushpinViewModel>(OnDeselectPushpin);
			CommandDeletePlace = new DelegateCommand<PushpinViewModel>(OnDeletePlace);
			CommandPinSearchResult = new DelegateCommand<PushpinViewModel>(OnPinSearchResult);
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the collection of all user pins.
		/// </summary>
		public AdaptedObservableCollection<Place, PushpinViewModel> Pushpins { get; private set; }

		/// <summary>
		/// Gets or sets the selected place.
		/// </summary>
		public PushpinViewModel SelectedPushpin
		{
			get { return _selectedPushpin; }
			set
			{
				if (!SetProperty(ref _selectedPushpin, value, SelectedPushpinProperty))
				{
					return;
				}
				if (Pushpins == null)
				{
					return;
				}
				foreach (var pushpin in Pushpins)
				{
					if (value == null)
					{
						pushpin.HighlightState = PushpinHighlightStates.None;
						pushpin.VisualState = PushpinContentVisualStates.Collapsed;
					}
					else if (value == pushpin)
					{
						pushpin.HighlightState = PushpinHighlightStates.Highlighted;
						pushpin.VisualState = PushpinContentVisualStates.Expanded;
					}
					else
					{
						pushpin.HighlightState = PushpinHighlightStates.UnHighlighted;
						pushpin.VisualState = PushpinContentVisualStates.Collapsed;
					}
				}
			}
		}

		private PushpinViewModel _selectedPushpin;
		private const string SelectedPushpinProperty = "SelectedPushpin";

		/// <summary>
		/// Gets or sets the map center geo-coordination.
		/// </summary>
		public Location MapCenter
		{
			get { return _mapCenter; }
			set { SetProperty(ref _mapCenter, value, MapCenterProperty); }
		}

		private Location _mapCenter;
		private const string MapCenterProperty = "MapCenter";

		/// <summary>
		/// Sets the function that returns the current view port of the map.
		/// </summary>
		public Func<LocationRect> MapBoundCapturer { private get; set; }

		#endregion


		#region Commands

		/// <summary>
		/// Gets the command that adds a user pin.
		/// </summary>
		public DelegateCommand<Location> CommandAddPlace { get; private set; }

		/// <summary>
		/// Gets the command that toggles the pushpin content state.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandToggleSelectedPushpin { get; private set; }

		/// <summary>
		/// Gets the command that collapses the user pin.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandDeselectPushpin { get; private set; }

		/// <summary>
		/// Gets the command deletes a user pin.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandDeletePlace { get; private set; }

		/// <summary>
		/// Gets the command that pins a search result.
		/// </summary>
		public DelegateCommand<PushpinViewModel> CommandPinSearchResult { get; private set; }

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="CommandToggleSelectedPushpin"/> is executed.
		/// </summary>
		private void OnToggleSelectedPushpin(PushpinViewModel pushpin)
		{
			SelectedPushpin = SelectedPushpin == pushpin ? null : pushpin;
		}

		/// <summary>
		/// Called when <see cref="CommandAddPlace"/> is executed.
		/// </summary>
		private void OnAddPlace(Location location)
		{
			var place = _data.AddNewPlace(location);
			var vm = Pushpins.LastOrDefault();
			if (vm == null)
			{
				return;
			}
			vm.WorkingState = PushpinContentWorkingStates.Working;
			var bounds = MapBoundCapturer.ExecuteIfNotNull();
			_data.GetPlaceInformation(
				place,
				bounds,
				args => vm.WorkingState = args.Status == CallbackStatus.Successful ? PushpinContentWorkingStates.Idle : PushpinContentWorkingStates.Error);
		}

		/// <summary>
		/// Called when <see cref="CommandDeselectPushpin"/> is executed.
		/// </summary>
		private void OnDeselectPushpin(PushpinViewModel vm)
		{
			if (SelectedPushpin != vm)
			{
				return;
			}
			SelectedPushpin = null;
		}

		/// <summary>
		/// Called when <see cref="CommandDeletePlace"/> is executed.
		/// </summary>
		private void OnDeletePlace(PushpinViewModel vm)
		{
			_data.RemovePlace(vm.Data);
		}

		/// <summary>
		/// Called when <see cref="CommandPinSearchResult"/> is executed.
		/// </summary>
		private void OnPinSearchResult(PushpinViewModel vm)
		{
			vm.Data.IsSearchResult = false;
		}

		/// <summary>
		/// Called when items in <see cref="Pushpins"/> collection are changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void OnPushpinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
					foreach (var pushpin in e.OldItems.Cast<PushpinViewModel>())
					{
						if (SelectedPushpin == pushpin)
						{
							SelectedPushpin = null;
						}
						pushpin.Dispose();
					}
					break;
			}
		}

		protected override void OnDispose()
		{
			Pushpins.CollectionChanged -= OnPushpinsCollectionChanged;
			Pushpins = null;
			SelectedPushpin = null;
			MapBoundCapturer = null;

			base.OnDispose();
		}

		#endregion
	}
}

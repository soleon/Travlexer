using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codify.Extensions;
using Microsoft.Phone.Shell;

namespace Codify.DependencyShell
{
	public abstract class ApplicationBarItemBase<T> : FrameworkElement where T: class, IApplicationBarMenuItem
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the real item that is displayed in the real shell container.
		/// </summary>
		internal T Item { get; set; }

		/// <summary>
		/// The string to display on the menu item.
		/// </summary>
		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",
			typeof (string),
			typeof(ApplicationBarItemBase<T>),
			new PropertyMetadata(null, OnTextChanged));

		public bool IsEnabled
		{
			get { return (bool) GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
			"IsEnabled",
			typeof (bool),
			typeof (ApplicationBarItemBase<T>),
			new PropertyMetadata(true, OnIsEnabledChanged));

		public ICommand Command
		{
			get { return (ICommand) GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			"Command",
			typeof (ICommand),
			typeof (ApplicationBarItemBase<T>),
			null);

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
			"CommandParameter",
			typeof (object),
			typeof (ApplicationBarItemBase<T>),
			null);

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="Text"/> is changed.
		/// </summary>
		private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var b = (ApplicationBarItemBase<T>)sender;
			if (b.Item == null)
			{
				return;
			}
			var text = (string) e.NewValue;
			b.Item.Text = text;
		}

		/// <summary>
		/// Called when <see cref="Control.IsEnabled"/> is changed.
		/// </summary>
		private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var b = (ApplicationBarItemBase<T>)sender;
			if (b.Item == null)
			{
				return;
			}
			var isEnabled = (bool) e.NewValue;
			b.Item.IsEnabled = isEnabled;
		}

		/// <summary>
		/// Called when any event of the item is raised that requires executing the <see cref="Command"/>.
		/// </summary>
		protected void OnExecuteCommand(object sender, EventArgs e)
		{
			Command.ExecuteIfNotNull(CommandParameter);
		}

		#endregion
	}
}

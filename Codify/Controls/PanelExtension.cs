using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Codify.Controls
{
	public class PanelExtension
	{
		private Panel _panel;

		public static INotifyCollectionChanged GetItemsSource(DependencyObject obj)
		{
			return (INotifyCollectionChanged)obj.GetValue(ItemsSourceProperty);
		}

		public static void SetItemsSource(DependencyObject obj, INotifyCollectionChanged value)
		{
			obj.SetValue(ItemsSourceProperty, value);
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
			"ItemsSource",
			typeof(INotifyCollectionChanged),
			typeof(PanelExtension),
			new PropertyMetadata(default(INotifyCollectionChanged), OnItemsSourceChanged));

		private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var panel = sender as Panel;
			if (panel == null)
			{
				throw new InvalidOperationException("Codify.Controls.PanelExtension.ItemsSource property can only be attached to a System.Windows.Controls.Panel.");
			}

			var ext = GetExtension(panel);
			if (ext == null)
			{
				SetExtension(panel, ext = new PanelExtension());
			}

			var oldSource = e.OldValue as INotifyCollectionChanged;
			var newSource = e.NewValue as INotifyCollectionChanged;

			if (oldSource != null)
			{
				oldSource.CollectionChanged -= ext.OnItemsSourceCollectionChanged;
				panel.Children.Clear();
			}
			if (newSource != null)
			{
				newSource.CollectionChanged += ext.OnItemsSourceCollectionChanged;
				ext.AddItems(newSource as IEnumerable);
			}
		}

		private static PanelExtension GetExtension(DependencyObject obj)
		{
			return (PanelExtension)obj.GetValue(_extensionProperty);
		}

		private static void SetExtension(DependencyObject obj, PanelExtension value)
		{
			obj.SetValue(_extensionProperty, value);
		}

		private static readonly DependencyProperty _extensionProperty = DependencyProperty.RegisterAttached(
			"Extension",
			typeof(PanelExtension),
			typeof(PanelExtension),
			new PropertyMetadata(default(PanelExtension), OnExtensionChanged));

		private static void OnExtensionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var panel = (Panel)sender;
			var oldExt = (PanelExtension)e.OldValue;
			var newExt = (PanelExtension)e.NewValue;
			if (oldExt != null)
			{
				oldExt._panel = null;
			}
			if (newExt != null)
			{
				newExt._panel = panel;
			}
		}

		public static DataTemplate GetItemTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(ItemTemplateProperty);
		}

		public static void SetItemTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(ItemTemplateProperty, value);
		}

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached(
			"ItemTemplate",
			typeof(DataTemplate),
			typeof(PanelExtension),
			null);

		private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_panel == null)
			{
				return;
			}
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						AddItems(e.NewItems);
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						var oldItems = e.OldItems;
						var children = _panel.Children;
						var uiChildren = children.Where(elm => elm is FrameworkElement).Cast<FrameworkElement>().ToList();
						if (!uiChildren.Any())
						{
							return;
						}
						foreach (var child in from object item in oldItems from child in uiChildren where child.DataContext == item select child)
						{
							children.Remove(child);
						}
						break;
					}
				case NotifyCollectionChangedAction.Replace:
					throw new NotSupportedException();
				case NotifyCollectionChangedAction.Reset:
					_panel.Children.Clear();
					break;
			}
		}

		private void AddItems(IEnumerable items)
		{
			if (items == null)
			{
				return;
			}
			var template = GetItemTemplate(_panel);
			if (template == null)
			{
				return;
			}
			foreach (var item in items)
			{
				var element = template.LoadContent() as UIElement;
				if (element == null)
				{
					return;
				}
				var frameworkElement = element as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.DataContext = item;
				}
				_panel.Children.Add(element);
			}
		}
	}
}

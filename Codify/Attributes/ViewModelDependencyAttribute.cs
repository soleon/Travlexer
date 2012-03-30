using System;

namespace Codify.Attributes
{
	public class ViewModelTypeAttribute : Attribute
	{
		public ViewModelTypeAttribute(Type viewModelType)
		{
			Type = viewModelType;
		}

		public Type Type { get; private set; }
	}
}

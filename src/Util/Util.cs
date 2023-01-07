using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ElfBot;

/// <summary>
/// Base class definition for a class to extend if it needs to perform
/// two-way binding with the WPF XAML application.
/// </summary>
public abstract class PropertyNotifyingClass : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// Invoked by a property that has change, causes any XAML
	/// that is bound to the property to refresh itself.
	/// </summary>
	/// <param name="propertyName">Name of the property that was changed</param>
	protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
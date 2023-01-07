using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

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

[ValueConversion(typeof(float), typeof(String))]
public class FloatFormattingConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return $"{value:n0}";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return float.Parse(value as string ?? string.Empty);
	}
}


[ValueConversion(typeof(object), typeof(String))]
public class NullValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value?.ToString() ?? "N/A";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ElfBot;

using Trace = System.Diagnostics.Trace;
using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;


public static class Util
{
	public static void LogConfigSetFloat(string name, float val)
	{
		Trace.WriteLine($"Config Set Float Value: {name} : {val}");
		Globals.Logger.Log(Level.Debug, $"Config Set Value {name} : {val}");
	}

	public static void LogConfigSetBool(string name, bool val)
	{
		Trace.WriteLine($"Config Set Bool Value: {name} : {val}");
		Globals.Logger.Log(Level.Debug, $"Config Set Bool Value {name} : {val}");
	}

	public static void LogConfigSetKey(string name, VirtualKeyCode key)
	{
		Trace.WriteLine($"Config Added Key: {name} : {key}");
		Globals.Logger.Log(Level.Debug, $"Config Added Key: {name} : {key}");
	}
}

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
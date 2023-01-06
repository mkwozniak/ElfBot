namespace ElfBot;

using Trace = System.Diagnostics.Trace;
using TextBox = System.Windows.Controls.TextBox;
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

	/// <summary> Tries to parse a float in an input box and stores the value in the referenced float. </summary>
	/// <param name="box"> The box to try to parse the float. </param>
	/// <param name="write"> The float to store the parsed value. </param>
	/// <param name="isPercentage"> If true, forces the value to be within 0 - 100 </param>
	public static void TryFloatFromInputBox(TextBox box, ref float write, bool isPercentage = false)
	{
		float result = 0f;

		if (!float.TryParse(box.Text, out result))
		{
			box.Text = write.ToString();
			return;
		}

		if (isPercentage && (result <= 0 || result > 100))
		{
			return;
		}

		write = result;
	}
}
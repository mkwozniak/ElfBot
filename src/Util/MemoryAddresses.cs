using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ElfBot.Util;

/// <summary>
/// Represents an address in memory at a given offset.
/// </summary>
public interface IMemoryAddress
{
	public string App { get; }

	public string Address { get; }

	public int[] Offsets { get; }

	/// <summary>
	/// Creates a formatted string containing the application name and
	/// offsets.
	///
	/// Resulting format is {app}+{offset1,offset2,...}.
	/// </summary>
	/// <param name="app">application name</param>
	/// <param name="offsets">offsets</param>
	/// <returns>formatted address value</returns>
	public static string CreateAddress(string app, int[] offsets)
	{
		var offsetStrings = offsets.Select(o => o.ToString("X4"));
		return $"{app}+{string.Join(",", offsetStrings)}";
	}
}

/// <summary>
/// Standard memory address implementation, holding an application
/// name and a set of offsets to append.
/// </summary>
public class MemoryAddress : IMemoryAddress
{
	public string App { get; }
	public int[] Offsets { get; }

	public string Address => IMemoryAddress.CreateAddress(App, Offsets);

	public MemoryAddress(string app, params int[] offsets)
	{
		if (string.IsNullOrEmpty(app))
		{
			throw new ArgumentException("Cannot provide null/empty 'app' or 'address' parameter");
		}

		if (offsets.Length == 0)
		{
			throw new ArgumentException("Must provide at least 1 offset");
		}

		App = app;
		Offsets = offsets;
	}
}

/// <summary>
/// Utility class used to wrap an existing memory address and provide
/// additional offsets. Useful when wanting to reuse an existing base
/// memory address value and append more offsets.
/// </summary>
public class WrappedMemoryAddress : IMemoryAddress
{
	private readonly IMemoryAddress _wrapped;
	private readonly int[] _offsets;

	public string App => _wrapped.App;

	public int[] Offsets => _wrapped.Offsets.Concat(_offsets).ToArray();

	public string Address => IMemoryAddress.CreateAddress(App, Offsets);

	public WrappedMemoryAddress(IMemoryAddress wrapped, params int[] offsets)
	{
		if (offsets.Length == 0)
		{
			throw new ArgumentException("Must provide at least 1 offset");
		}

		_wrapped = wrapped ?? throw new ArgumentException("Wrapped memory address cannot be null");
		_offsets = offsets;
	}
}

/// <summary>
/// Represents a value of a given type stored at an address in memory. 
/// </summary>
/// <typeparam name="T">Value type stored in memory</typeparam>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public abstract class AddressValue<T>
{
	protected readonly IMemoryAddress Address;

	protected AddressValue(IMemoryAddress address)
	{
		Address = address;
	}

	/// <summary>
	/// Gets the stored value.
	/// </summary>
	/// <returns>Corresponding type</returns>
	public abstract T GetValue();

	/// <summary>
	/// Writes a value to the address stored by the field.
	/// </summary>
	/// <param name="value">Value to write into memory</param>
	/// <returns></returns>
	public abstract bool WriteValue(T value);
}

/// <summary>
/// Value at an address which contains a string.
/// </summary>
public class StringValue : AddressValue<string>
{
	public override string GetValue()
	{
		return MainWindow.TargetApplicationMemory.ReadString(Address.Address);
	}

	public override bool WriteValue(string value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "string", value);
	}

	public StringValue(IMemoryAddress address) : base(address)
	{
	}
}

/// <summary>
/// Value at an address which contains an int value (4 bytes).
/// </summary>
public class IntValue : AddressValue<int>
{
	public override int GetValue()
	{
		return MainWindow.TargetApplicationMemory.ReadInt(Address.Address);
	}

	public override bool WriteValue(int value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "int", value.ToString());
	}

	public IntValue(IMemoryAddress address) : base(address)
	{
	}
}

/// <summary>
/// Value at an address which contains an int value (4 bytes).
/// </summary>
public class LongValue : AddressValue<long>
{
	public override long GetValue()
	{
		return MainWindow.TargetApplicationMemory.ReadLong(Address.Address);
	}

	public override bool WriteValue(long value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "long", value.ToString());
	}

	public LongValue(IMemoryAddress address) : base(address)
	{
	}
}

/// <summary>
/// Value at an address which contains a float value (4 bytes).
/// </summary>
public class FloatValue : AddressValue<float>
{
	public override float GetValue()
	{
		return MainWindow.TargetApplicationMemory.ReadFloat(Address.Address);
	}

	public override bool WriteValue(float value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "float", value.ToString());
	}

	public FloatValue(IMemoryAddress address) : base(address)
	{
	}
}

/// <summary>
/// Value at an address which contains a short value (2 bytes).
/// </summary>
public class TwoByteValue : AddressValue<int>
{
	public override int GetValue()
	{
		return MainWindow.TargetApplicationMemory.Read2Byte(Address.Address);
	}

	public override bool WriteValue(int value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "2bytes", value.ToString());
	}

	public TwoByteValue(IMemoryAddress address) : base(address)
	{
	}
}

/// <summary>
/// Value at an address which contains a byte value.
/// </summary>
public class ByteValue : AddressValue<byte>
{
	public override byte GetValue()
	{
		return (byte) MainWindow.TargetApplicationMemory.ReadByte(Address.Address);
	}

	public override bool WriteValue(byte value)
	{
		return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "byte", value.ToString());
	}

	public ByteValue(IMemoryAddress address) : base(address)
	{
	}
}
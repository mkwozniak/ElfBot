using System.Linq;

namespace ElfBot
{
	using System;

	public static class GlobalAddresses
	{
		public static Offset PlayerObjectOffset => new(0x10BE100);
		public static Offset NoClipOffset => new(0xB4D70);

		public static readonly string ProcessName = "trose.exe";
	}

	public static class Addresses
	{
		public static int NoClipOffset = 0xB4D70;

		// General Character Information
		public static readonly StringField CharacterName = new(new MemoryAddress("trose.exe", new Offset(0x10C1918)));

		public static readonly TwoByteField Level =
			new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x3AD8)));

		public static readonly IntField Xp = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x3AD4)));

		public static readonly IntField Zuly = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x3D38)));

		// Player Stats
		public static readonly IntField Hp = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x3ACC)));

		public static readonly IntField MaxHp = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x4600)));

		public static readonly IntField Mp = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x3AD0)));

		public static readonly IntField MaxMp = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10BE100, 0x4604)));

		// Position
		public static readonly FloatField PositionX = new(new MemoryAddress("trose.exe",
			Offset.FromInt(0x10BE100, 0x258, 0x370, 0xA0, 0x380, 0x1B8)));

		public static readonly FloatField PositionY = new(new MemoryAddress("trose.exe",
			Offset.FromInt(0x10BE100, 0x258, 0x370, 0xA0, 0x380, 0x1BC)));

		public static readonly FloatField PositionZ = new(new MemoryAddress("trose.exe",
			Offset.FromInt(0x10BE100, 0x258, 0x370, 0xA0, 0x380, 0x1C0)));

		// Camera
		public static readonly FloatField CameraZoom =
			new(new MemoryAddress("trose.exe", Offset.FromInt(0x10D2520, 0xD70, 0x6C4)));

		public static readonly FloatField CameraPitch =
			new(new MemoryAddress("trose.exe", Offset.FromInt(0x010D2520, 0xD70, 0x6C0)));

		public static readonly FloatField CameraYaw =
			new(new MemoryAddress("trose.exe", Offset.FromInt(0x010D2520, 0xD70, 0x6BC)));

		// Misc
		public static readonly IntField MapId = new(new MemoryAddress("trose.exe", new Offset(0x10C4AE4)));

		public static readonly StringField Target = new(new MemoryAddress("trose.exe", new Offset(0x10D8C10)));

		public static readonly IntField TargetId = new(new MemoryAddress("trose.exe", Offset.FromInt(0x10C0458, 0x8)));
	}

	public class Entity
	{
		private readonly FloatField _posXField;
		private readonly FloatField _posYField;
		private readonly FloatField _posZField;
		private readonly IntField _idField;

		public float PositionX => _posXField.GetValue();

		public float PositionY => _posYField.GetValue();

		public float PositionZ => _posZField.GetValue();

		public int Id => _idField.GetValue();

		protected Entity(Offset[] baseOffset) : this(new MemoryAddress(GlobalAddresses.ProcessName, baseOffset))
		{
		}

		protected Entity(IMemoryAddress baseAddress)
		{
			_idField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x1C)));
			_posXField = new(new WrappedMemoryAddress(baseAddress, Offset.FromInt(0x258, 0x370, 0xA0, 0x380, 0x1B8)));
			_posYField = new(new WrappedMemoryAddress(baseAddress, Offset.FromInt(0x258, 0x370, 0xA0, 0x380, 0x1BC)));
			_posZField = new(new WrappedMemoryAddress(baseAddress, Offset.FromInt(0x258, 0x370, 0xA0, 0x380, 0x1C0)));
		}
	}

	public class TargetedEntity : Entity
	{
		private readonly IntField _hpField;
		private readonly IntField _mpField;
		private readonly IntField _maxHpField;
		private readonly IntField _maxMpField;

		public int Hp => _hpField.GetValue();
		public int MaxHp => _maxHpField.GetValue();
		public int Mp => _mpField.GetValue();
		public int MaxMp => _maxMpField.GetValue();

		public TargetedEntity(int id) : this(_createBaseOffset(id))
		{
		}

		private TargetedEntity(Offset[] baseOffset) : this(new MemoryAddress(GlobalAddresses.ProcessName, baseOffset))
		{
		}

		private TargetedEntity(IMemoryAddress baseAddress) : base(baseAddress)
		{
			_hpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0xE8)));
			_mpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0xEC)));
			_maxHpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0xF0)));
			_maxMpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0xF4)));
		}

		private static Offset[] _createBaseOffset(int id)
		{
			return new[] { new Offset((id * 8) + 0x22078) };
		}
	}

	public class Player : Entity
	{
		private readonly StringField _nameField;
		private readonly TwoByteField _levelField;
		private readonly IntField _xpField;
		private readonly IntField _zulyField;
		private readonly IntField _hpField;
		private readonly IntField _maxHpField;
		private readonly IntField _mpField;
		private readonly IntField _maxMpField;

		public string Name => _nameField.GetValue();
		public int Level => _levelField.GetValue();
		public int Xp => _xpField.GetValue();
		public int Zuly => _zulyField.GetValue();
		public int Hp => _hpField.GetValue();
		public int MaxHp => _maxHpField.GetValue();
		public int Mp => _mpField.GetValue();
		public int MaxMp => _maxMpField.GetValue();

		public Player() : this(new MemoryAddress(GlobalAddresses.ProcessName, GlobalAddresses.PlayerObjectOffset))
		{
		}

		private Player(IMemoryAddress baseAddress) : base(baseAddress)
		{
			_nameField = new(new WrappedMemoryAddress(baseAddress, new Offset(0xB10)));
			_levelField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x3AD8)));
			_xpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x3AD4)));
			_zulyField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x3D38)));
			_hpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x3ACC)));
			_mpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x4600)));
			_maxHpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x3AD0)));
			_maxMpField = new(new WrappedMemoryAddress(baseAddress, new Offset(0x4604)));
		}
	}

	public class Offset
	{
		public int Value { get; }

		public Offset(int value)
		{
			Value = value;
		}

		public static Offset[] FromInt(params int[] offsets)
		{
			return offsets.Select(o => new Offset(o)).ToArray();
		}
	}

	public interface IMemoryAddress
	{
		public string App { get; }

		public String Address { get; }

		public Offset[] Offsets { get; }

		public static string CreateAddress(string app, Offset[] offsets)
		{
			var offsetStrings = offsets.Select(o => o.Value.ToString("X4"));
			return $"{app}+{string.Join(",", offsetStrings)}";
		}
	}

	public class WrappedMemoryAddress : IMemoryAddress
	{
		private IMemoryAddress _wrapped;
		private Offset[] _offsets;

		public string App => _wrapped.App;

		public Offset[] Offsets => _wrapped.Offsets.Union(_offsets).ToArray();

		public string Address => IMemoryAddress.CreateAddress(App, Offsets);

		public WrappedMemoryAddress(IMemoryAddress wrapped, params Offset[] offsets)
		{
			if (offsets.Length == 0)
			{
				throw new ArgumentException("Must provide at least 1 offset");
			}

			_wrapped = wrapped ?? throw new ArgumentException("Wrapped memory address cannot be null");
			_offsets = offsets;
		}
	}

	// Need a dynamic and a static list

	public class MemoryAddress : IMemoryAddress
	{
		public string App { get; }
		public Offset[] Offsets { get; }

		public string Address => IMemoryAddress.CreateAddress(App, Offsets);

		public MemoryAddress(string app, params Offset[] offsets)
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
	/// Represents a value of a given type stored in memory. 
	/// </summary>
	/// <typeparam name="T">Value type stored in memory</typeparam>
	public abstract class AddressField<T>
	{
		protected IMemoryAddress Address;

		protected AddressField(IMemoryAddress address)
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

	public class StringField : AddressField<string>
	{
		public override string GetValue()
		{
			return MainWindow.TargetApplicationMemory.ReadString(Address.Address);
		}

		public override bool WriteValue(string value)
		{
			return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "string", value);
		}

		public StringField(IMemoryAddress address) : base(address)
		{
		}
	}

	public class IntField : AddressField<int>
	{
		public override int GetValue()
		{
			return MainWindow.TargetApplicationMemory.ReadInt(Address.Address);
		}

		public override bool WriteValue(int value)
		{
			return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "int", value.ToString());
		}

		public IntField(IMemoryAddress address) : base(address)
		{
		}
	}

	public class FloatField : AddressField<float>
	{
		public override float GetValue()
		{
			return MainWindow.TargetApplicationMemory.ReadFloat(Address.Address);
		}

		public override bool WriteValue(float value)
		{
			return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "float", value.ToString());
		}

		public FloatField(IMemoryAddress address) : base(address)
		{
		}
	}

	public class TwoByteField : AddressField<int>
	{
		public override int GetValue()
		{
			return MainWindow.TargetApplicationMemory.Read2Byte(Address.Address);
		}

		public override bool WriteValue(int value)
		{
			return MainWindow.TargetApplicationMemory.WriteMemory(Address.Address, "2bytes", value.ToString());
		}

		public TwoByteField(IMemoryAddress address) : base(address)
		{
		}
	}
}
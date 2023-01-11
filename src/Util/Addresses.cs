namespace ElfBot
{
    using System;
    using System.Diagnostics;
    using System.Printing.IndexedProperties;

    public static class Addresses
    {
        // General Character Information
        public static readonly StringField CharacterName = new StringField("trose.exe", "10C1918");//updated 2023-01-10
        public static readonly TwoByteField Level = new TwoByteField("trose.exe", "10BE100", "0x3AD8");//updated 2023-01-10
		public static readonly IntField Xp = new IntField("trose.exe", "10BE100", "0x3AD4");//updated 2023-01-10
		public static readonly IntField Zuly = new IntField("trose.exe", "10BE100", "0x3D38");//updated 2023-01-10
		// Player Stats
		public static readonly IntField Hp = new IntField("trose.exe", "10BE100", "0x3ACC");//updated 2023-01-10
		public static readonly IntField MaxHp = new IntField("trose.exe", "10BE100", "0x4600");//updated 2023-01-10
		public static readonly IntField Mp = new IntField("trose.exe", "10BE100", "0x3AD0");//updated 2023-01-10
		public static readonly IntField MaxMp = new IntField("trose.exe", "10BE100", "0x4604");//updated 2023-01-10
        // Position
        public static readonly FloatField PositionX =
            new FloatField("trose.exe", "010BE100", "0x258", "0x370", "0xA0", "0x380", "0x1B8");//updated 2023-01-10
		public static readonly FloatField PositionY =
            new FloatField("trose.exe", "010BE100", "0x258", "0x370", "0xA0", "0x380", "0x1BC");//updated 2023-01-10
		public static readonly FloatField PositionZ =
            new FloatField("trose.exe", "010BE100", "0x258", "0x370", "0xA0", "0x380", "0x1C0");//updated 2023-01-10
        // Camera
        public static readonly FloatField CameraZoom = new FloatField("trose.exe", "010D2520", "0xD70", "0x6C4");//updated 2023-01-10
		public static readonly FloatField CameraPitch = new FloatField("trose.exe", "010D2520", "0xD70", "0x6C0");//updated 2023-01-10
		public static readonly FloatField CameraYaw = new FloatField("trose.exe", "010D2520", "0xD70", "0x6BC");//updated 2023-01-10
		// Misc
		public static readonly IntField MapId = new IntField("trose.exe", "10C4AE4");//updated 2023-01-10
        public static readonly StringField Target = new StringField("trose.exe", "10D8C10");//updated 2023-01-10
		public static readonly IntField TargetId = new IntField("trose.exe", "10C0458", "0x8");//updated 2023-01-10
		public static readonly StringField TargetDefeatedMessage = new StringField("trose.exe", "10C5950");//updated 2023-01-10

        public static readonly ByteArrayField NoClipOn = new ByteArrayField(new byte[] { 0xC3, 0x90 }); // updated 2023-01-11
		public static readonly ByteArrayField NoClipOff = new ByteArrayField(new byte[] { 0x40, 0x57 }); // updated 2023-01-11
	}

    /// <summary>
    /// Represents a value of a given type stored in memory. 
    /// </summary>
    /// <typeparam name="T">Value type stored in memory</typeparam>
    public abstract class AddressField<T>
    {
        private string app;
        private string[] offsets;

        protected string Address => $"{app}+{string.Join(",", offsets)}";

        protected AddressField(string app, params string[] offsets)
        {
            if (string.IsNullOrEmpty(app))
            {
                throw new ArgumentException("Cannot provide null/empty 'app' or 'address' parameter");
            }

            if (offsets.Length == 0)
            {
                throw new ArgumentException("Must provide at least 1 offset");
            }

            this.app = app;
            this.offsets = offsets;
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
        public abstract bool writeValue(T value);
    }

    public class StringField : AddressField<string>
    {
        public override string GetValue()
        {
            return MainWindow.TargetApplicationMemory.ReadString(Address);
        }

        public override bool writeValue(string value)
        {
			return MainWindow.TargetApplicationMemory.WriteMemory(Address, "string", value);
        }

        public StringField(string app, params string[] offsets) : base(app, offsets)
        {
        }
    }

    public class IntField : AddressField<int>
    {
        public override int GetValue()
        {
            return MainWindow.TargetApplicationMemory.ReadInt(Address);
        }

        public override bool writeValue(int value)
        {
            return MainWindow.TargetApplicationMemory.WriteMemory(Address, "int", value.ToString());
        }

        public IntField(string app, params string[] offsets) : base(app, offsets)
        {
        }
    }

    public class FloatField : AddressField<float>
    {
        public override float GetValue()
        {
            return MainWindow.TargetApplicationMemory.ReadFloat(Address);
        }

        public override bool writeValue(float value)
        {
            return MainWindow.TargetApplicationMemory.WriteMemory(Address, "float", value.ToString());
        }

        public FloatField(string app, params string[] offsets) : base(app, offsets)
        {
        }
    }

    public class TwoByteField : AddressField<int>
    {
        public override int GetValue()
        {
            return MainWindow.TargetApplicationMemory.Read2Byte(Address);
        }

        public override bool writeValue(int value)
        {
            return MainWindow.TargetApplicationMemory.WriteMemory(Address, "2bytes", value.ToString());
        }

        public TwoByteField(string app, params string[] offsets) : base(app, offsets)
        {
        }
    }

	public class ByteArrayField
	{
        private byte[] _bytes;

		public ByteArrayField(byte[] bytes)
		{
            _bytes = bytes;
		}

		public byte[] GetValue()
		{
			return _bytes;
		}

		public bool Write(Int64 address)
		{
            if (_bytes == null)
                return false;

			int bytesWritten = 0;

			RoseProcess.WriteBytes((IntPtr)address, _bytes, ref bytesWritten);

			Trace.WriteLine("NoClip Test Bytes Written: " + bytesWritten);

			if (bytesWritten > 0)
				return true;

            return false;
		}
	}
}

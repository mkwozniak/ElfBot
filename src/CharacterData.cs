using System;

namespace ElfBot;

public class CharacterData : PropertyNotifyingClass
{
	private string? _name;
	private int _level;
	private int _xp;
	private int _zuly;
	private int _hp, _maxHp;
	private int _mp, _maxMp;
	public Location Location { get; } = new();
	public CameraOrientation Camera { get; } = new();
	public CurrentTarget CurrentTarget { get; } = new();

	public string? Name
	{
		get => _name;
		private set
		{
			if (_name == value) return;
			_name = value;
			NotifyPropertyChanged();
		}
	}

	public int Level
	{
		get => _level;
		private set
		{
			if (_level == value) return;
			_level = value;
			NotifyPropertyChanged();
		}
	}

	public int Xp
	{
		get => _xp;
		private set
		{
			if (_xp == value) return;
			_xp = value;
			NotifyPropertyChanged();
		}
	}

	public int Zuly
	{
		get => _zuly;
		private set
		{
			if (_zuly == value) return;
			_zuly = value;
			NotifyPropertyChanged();
		}
	}

	public int Hp
	{
		get => _hp;
		private set
		{
			if (_hp == value) return;
			_hp = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged("HpText");
		}
	}

	public int MaxHp
	{
		get => _maxHp;
		private set
		{
			if (_maxHp == value) return;
			_maxHp = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged("HpText");
		}
	}

	public string HpText => $"{Hp} / {MaxHp}";

	public int Mp
	{
		get => _mp;
		private set
		{
			if (_mp == value) return;
			_mp = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged("MpText");
		}
	}

	public int MaxMp
	{
		get => _maxMp;
		private set
		{
			if (_maxMp == value) return;
			_maxMp = value;
			NotifyPropertyChanged();
			NotifyPropertyChanged("MpText");
		}
	}

	public string MpText => $"{Mp} / {MaxMp}";

	public void Update()
	{
		Name = Addresses.CharacterName.GetValue();
		Level = Addresses.Level.GetValue();
		Xp = Addresses.Xp.GetValue();
		Zuly = Addresses.Zuly.GetValue();
		Hp = Addresses.Hp.GetValue();
		MaxHp = Addresses.MaxHp.GetValue();
		Mp = Addresses.Mp.GetValue();
		MaxMp = Addresses.MaxMp.GetValue();
		Location.Update();
		Camera.Update();
		CurrentTarget.Update();
	}
}

public class Location : PropertyNotifyingClass
{
	private float _x, _y, _z;
	private int _mapId;

	public float X
	{
		get => _x;
		private set
		{
			if (Math.Abs(_x - value) < 0.01) return;
			_x = value;
			NotifyPropertyChanged();
		}
	}

	public float Y
	{
		get => _y;
		private set
		{
			if (Math.Abs(_y - value) < 0.01) return;
			_y = value;
			NotifyPropertyChanged();
		}
	}

	public float Z
	{
		get => _z;
		private set
		{
			if (Math.Abs(_z - value) < 0.01) return;
			_z = value;
			NotifyPropertyChanged();
		}
	}

	public int MapId
	{
		get => _mapId;
		private set
		{
			if (_mapId == value) return;
			_mapId = value;
			NotifyPropertyChanged();
		}
	}

	public void Update()
	{
		X = Addresses.PositionX.GetValue();
		Y = Addresses.PositionY.GetValue();
		Z = Addresses.PositionZ.GetValue();
		MapId = Addresses.MapId.GetValue();
	}
}

public class CameraOrientation : PropertyNotifyingClass
{
	private float _zoom, _pitch, _yaw;

	public float Zoom
	{
		get => _zoom;
		private set
		{
			if (Math.Abs(_zoom - value) < 0.01) return;
			_zoom = value;
			NotifyPropertyChanged();
		}
	}

	public float Pitch
	{
		get => _pitch;
		private set
		{
			if (Math.Abs(_pitch - value) < 0.01) return;
			_pitch = value;
			NotifyPropertyChanged();
		}
	}

	public float Yaw
	{
		get => _yaw;
		private set
		{
			if (Math.Abs(_yaw - value) < 0.01) return;
			_yaw = value;
			NotifyPropertyChanged();
		}
	}

	public void Update()
	{
		Zoom = Addresses.CameraZoom.GetValue();
		Pitch = Addresses.CameraPitch.GetValue();
		Yaw = Addresses.CameraYaw.GetValue();
	}
}

public class CurrentTarget : PropertyNotifyingClass
{
	private string _name;
	private int _id;

	public string Name
	{
		get => _name;
		private set
		{
			if (_name == value) return;
			_name = value;
			NotifyPropertyChanged();
		}
	}

	public int Id
	{
		get => _id;
		private set
		{
			if (_id == value) return;
			_id = value;
			NotifyPropertyChanged();
		}
	}

	public void Update()
	{
		Name = Addresses.Target.GetValue();
		Id = Addresses.TargetId.GetValue();
	}
}
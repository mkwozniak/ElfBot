using ElfBot.Util;

namespace ElfBot;

// @formatter:off
internal static class StaticOffsets
{
	public const int PlayerObject       = 0x10BE100;  // type: pointer
	public const int ClientCameraObject = 0x010D2520; // type: pointer
	public const int CurrentMapId       = 0x10C4AE4;  // type: 4 bytes/int
	public const int CurrentTarget      = 0x10C0458;  // type: pointer
	public const int CurrentTargetName  = 0x10D8C10;  // type: pointer, name at 0x8
	public const int EntityList         = 0x10C0FF0;  // type: ptr/list
	public const int NoClipFunction     = 0xB4D70;    // type: function, requires kernel32.dll
}
// @formatter:on

/// <summary>
/// Current method of transportation.
/// </summary>
// @formatter:off
public enum MoveMode
{
	Walk      = 0x00, // Walking (slowly)
	Run       = 0x01, // Running
	Driving   = 0x02, // Driving a vehicle or mount
	Passenger = 0x04  // Passenger of a vehicle (i.e., cart)
}
// @formatter:on

/// <summary>
/// Possible state of an entity.
/// </summary>
// @formatter:off
public enum State
{
	Idle               = 0x0000, // Not doing anything
	Moving             = 0x2101, // Moving to another location
	Attacking          = 0xD002, // Using normal attack
	TakingCriticalDmg  = 0xD003, // Received critical damage (in hit animation).
	SittingDown        = 0xD005, // Animation to sit down.
	Sitting            = 0x1006, // Seemingly unused - falling down animation.
	StandingUp         = 0xD007, // Animation to stand up.
	AttackWithSkill    = 0xD008, // Attacking with a skill.
	FinishSkillAttack  = 0xD209, // Skill animation ending.
	Dead               = 0xC010, // Currently dead.
	CastingSkill       = 0xD211  // Casting a non-attack skill such as a buff or summon.
}
// @formatter:on

/// <summary>
/// Current command an entity is executing.
/// </summary>
// @formatter:off
public enum Command
{
	None           = 0x0000, // No active command.
	Move           = 0x0001, // Moving to another location.
	Attack         = 0x0002, // Using normal attack
	Die            = 0x0003, // Dead
	LootItem       = 0x0004, // Looting an item
	SkillSelf      = 0x0006, // Casting a skill on self
	SkillTarget    = 0x0007, // Casting a skill on a target
	SkillPosition  = 0x0008, // Casting a skill on a position
	RunAway        = 0x8009, // Running away
	Sit            = 0x000a  // Sitting down
}
// @formatter:on

/// <summary>
/// Represents an entity in the game.
/// </summary>
public abstract class Entity
{
	private readonly FloatValue _rawPosXField;
	private readonly FloatValue _rawPosYField;
	private readonly FloatValue _rawPosZField;
	private readonly FloatValue _posXField;
	private readonly FloatValue _posYField;
	private readonly FloatValue _posZField;
	private readonly TwoByteValue _idField;
	private readonly TwoByteValue _currentStateField;
	private readonly TwoByteValue _currentCommandField;
	private readonly IntValue _activeObjectId;
	private readonly ByteValue _runMode;
	private readonly ByteValue _moveMode;

	public float PositionX => IsOnMount ? _rawPosXField.GetValue() / 100f : _posXField.GetValue();

	public float PositionY => IsOnMount ? _rawPosYField.GetValue() / 100f : _posYField.GetValue();

	public float PositionZ
	{
		get => IsOnMount ? _rawPosZField.GetValue() / 100f : _posZField.GetValue();
		set => _posZField.WriteValue(value);
	}

	public int Id => _idField.GetValue();

	public State State => (State)_currentStateField.GetValue();

	public Command CurrentCommand => (Command)_currentCommandField.GetValue();

	public bool IsDead => State == State.Dead || CurrentCommand == Command.Die;

	public bool IsSitting => State is State.SittingDown or State.Sitting or State.StandingUp
	                         || CurrentCommand == Command.Sit;

	public bool IsAttacking => State is State.Attacking or State.AttackWithSkill or State.FinishSkillAttack
	                           || CurrentCommand is Command.Attack;

	/// <summary>
	/// The ID of the last object or entity this entity interacted with.
	/// </summary>
	public int ActiveObjectId => _activeObjectId.GetValue();

	public bool IsRunning => _runMode.GetValue() == 0x01;

	public MoveMode MoveMode => (MoveMode)_moveMode.GetValue();

	public bool IsOnMount => MoveMode is MoveMode.Driving or MoveMode.Passenger;

	protected Entity(int[] baseOffset) : this(new MemoryAddress("trose.exe", baseOffset))
	{
	}

	protected Entity(IMemoryAddress baseAddress)
	{
		_idField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x1C));
		_rawPosXField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0x10));
		_rawPosYField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0x14));
		_rawPosZField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0x18));
		_posXField = new FloatValue(new WrappedMemoryAddress(baseAddress,
			0x258, 0x370, 0xA0, 0x380, 0x1B8));
		_posYField = new FloatValue(new WrappedMemoryAddress(baseAddress,
			0x258, 0x370, 0xA0, 0x380, 0x1BC));
		_posZField = new FloatValue(new WrappedMemoryAddress(baseAddress,
			0x258, 0x370, 0xA0, 0x380, 0x1C0));
		_currentStateField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x58));
		_currentCommandField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x5A));
		_activeObjectId = new IntValue(new WrappedMemoryAddress(baseAddress, 0x7C));
		_runMode = new ByteValue(new WrappedMemoryAddress(baseAddress, 0xC4));
		_moveMode = new ByteValue(new WrappedMemoryAddress(baseAddress, 0xC5));
	}

	/// <summary>
	/// Checks the desired ID for this monster against the ID
	/// sitting in memory and returns whether they match.
	/// </summary>
	/// <returns>whether this target entity is still valid or has been re-allocated</returns>
	public abstract bool IsValid();
}

/// <summary>
/// Current active character.
/// </summary>
public class Character : Entity
{
	private readonly StringValue _nameField;
	private readonly TwoByteValue _levelField;
	private readonly IntValue _xpField;
	private readonly IntValue _zulyField;
	private readonly IntValue _hpField;
	private readonly IntValue _maxHpField;
	private readonly IntValue _mpField;
	private readonly IntValue _maxMpField;
	private readonly IntValue _mapIdField;
	private readonly IntValue _targetIdField;
	private readonly StringValue _targetNameField;
	private readonly IntValue _consumedSummonsMeterField;

	private TargetedEntity? _target;

	public string Name => _nameField.GetValue();
	public int Level => _levelField.GetValue();
	public int Xp => _xpField.GetValue();
	public int Zuly => _zulyField.GetValue();
	public int Hp => _hpField.GetValue();
	public int MaxHp => _maxHpField.GetValue();
	public int Mp => _mpField.GetValue();
	public int MaxMp => _maxMpField.GetValue();
	public int MapId => _mapIdField.GetValue();

	public Camera Camera { get; } = new();

	public int LastTargetId
	{
		get => _targetIdField.GetValue();
		set => _targetIdField.WriteValue(value);
	}

	public string? TargetName => LastTargetId != 0 ? _targetNameField.GetValue() : null;

	public TargetedEntity? TargetEntity
	{
		get
		{
			var targetId = LastTargetId;
			if (_target == null && targetId <= 0) return null;

			// Reset the target entity if it doesn't exist, or the selected
			// ID has changed.
			if (_target == null || !_target.IsValid()
			                    || _target.Id != targetId)
			{
				_target = new TargetedEntity(targetId);
			}

			// Reset the target selection if the target entity is no 
			// longer valid.
			if (!_target.IsValid())
			{
				_target = null;
				LastTargetId = 0;
			}

			return _target;
		}
	}

	public int ConsumedSummonsMeter => _consumedSummonsMeterField.GetValue();

	public Character() : this(new MemoryAddress("trose.exe",
		StaticOffsets.PlayerObject))
	{
	}

	private Character(IMemoryAddress baseAddress) : base(baseAddress)
	{
		_nameField = new StringValue(new WrappedMemoryAddress(baseAddress, 0xB10));
		_levelField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x3AD8));
		_xpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3AD4));
		_zulyField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3D38));
		_hpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3ACC));
		_mpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3AD0));
		_maxHpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x4600));
		_maxMpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x4604));
		_mapIdField = new IntValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentMapId));
		_targetIdField = new IntValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentTarget, 0x8));
		_targetNameField = new StringValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentTargetName));
		_consumedSummonsMeterField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x57A0));
	}

	public override bool IsValid()
	{
		return Name.Trim().Length > 0;
	}

	public void ResetTargetMemory()
	{
		_targetIdField.WriteValue(0);
		_targetNameField.WriteValue("");
	}
}

/// <summary>
/// An entity that the character object has targeted.
/// </summary>
public class TargetedEntity : Entity
{
	private readonly IntValue _hpField;
	private readonly IntValue _mpField;
	private readonly IntValue _maxHpField;
	private readonly IntValue _maxMpField;

	private readonly int _originalId;

	public int Hp => _hpField.GetValue();
	public int MaxHp => _maxHpField.GetValue();
	public int Mp => _mpField.GetValue();
	public int MaxMp => _maxMpField.GetValue();

	public TargetedEntity(int id) : this(id,
		new MemoryAddress("trose.exe", StaticOffsets.EntityList, _createBaseOffset(id)))
	{
	}

	private TargetedEntity(int id, IMemoryAddress baseAddress) : base(baseAddress)
	{
		_originalId = id;
		_hpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0xE8));
		_mpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0xEC));
		_maxHpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0xF0));
		_maxMpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0xF4));
	}

	public override bool IsValid()
	{
		return Id == _originalId;
	}

	private static int _createBaseOffset(int id)
	{
		return (id * 8) + 0x22078;
	}
}

/// <summary>
/// Game camera view.
/// </summary>
public class Camera
{
	private readonly FloatValue _zoomField;
	private readonly FloatValue _pitchField;
	private readonly FloatValue _yawField;

	public float Zoom
	{
		get => _zoomField.GetValue();
		set => _zoomField.WriteValue(value);
	}

	public float Pitch
	{
		get => _pitchField.GetValue();
		set => _pitchField.WriteValue(value);
	}

	public float Yaw
	{
		get => _yawField.GetValue();
		set => _yawField.WriteValue(value);
	}

	public Camera()
	{
		MemoryAddress baseAddress = new("trose.exe", StaticOffsets.ClientCameraObject);
		_zoomField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0xD70, 0x6C4));
		_pitchField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0xD70, 0x6C0));
		_yawField = new FloatValue(new WrappedMemoryAddress(baseAddress, 0xD70, 0x6BC));
	}
}
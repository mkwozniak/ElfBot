using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using ElfBot.Util;
using Newtonsoft.Json;

namespace ElfBot;

// @formatter:off
internal static class StaticOffsets
{
	public const string ApplicationName = "trose.exe";
	public const int PlayerObject       = 0x10C61A0;  // type: pointer
	public const int PartyBase          = 0x10CC0B0;  // type: n/a, party information is relative to this address
	public const int ClientCameraObject = 0x10C4B98;  // type: pointer
	//public const int CurrentMapId       = 0x10C7BF8;  // type: 4 bytes/int
	public const int CurrentTarget      = 0x10C8500;  // type: pointer // trose.exe+1AF7FC
	public const int CurrentTargetName  = 0x10E0C70;  // type: string
	public const int EntityList         = 0x10C90A0;  // type: ptr/list
	public const int NoClipFunction     = 0xB55C0;    // type: function, requires kernel32.dll
	public const int ObjectMappings     = 0x10C90A0;  // type: pointer
}
// @formatter:on

//00007FF6F45BC0B0
// 7ff6f34f0000

public static class GameObjects
{
	private static NpcMappingTable _npcMappingTable = LoadNpcIdMap();
	public static int GetClientId(int serverId)
	{
		var address = new MemoryAddress(StaticOffsets.ApplicationName,
			StaticOffsets.ObjectMappings, (serverId * 2) + 0xC);
		return MainWindow.TargetApplicationMemory.Read2Byte(address.Address);
	}

	public static int GetServerId(int clientId)
	{
		var address = new MemoryAddress(StaticOffsets.ApplicationName,
			StaticOffsets.ObjectMappings, (clientId * 2) + 0x2000A);
		return MainWindow.TargetApplicationMemory.Read2Byte(address.Address);
	}

	public static string? GetNpcName(int id)
	{
		return _npcMappingTable.GetName(id);
	}

	public static List<TargetedEntity> GetVisibleMonsters()
	{
		List<TargetedEntity> monsters = new();
		for (var i = 0; i < 0x1000; i++)
		{
			var serverId = GetServerId(i);
			if (serverId == 0) continue;
			TargetedEntity ent = new TargetedEntity(i);
			if (ent.Type == ObjectType.Mob) monsters.Add(ent);
		}

		return monsters;
	}

	private static NpcMappingTable LoadNpcIdMap()
	{
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream("ElfBot.Assets.npc_list.json");

		if (stream == null) return new NpcMappingTable();

		using var reader = new StreamReader(stream);
		var json = reader.ReadToEnd();

		return JsonConvert.DeserializeObject<NpcMappingTable>(json)!;
	}
}

internal class NpcMappingTable
{
	[JsonProperty("id_to_lnpc")]
	private ImmutableDictionary<int, string> _idToLnpc = ImmutableDictionary<int, string>.Empty;
	[JsonProperty("lnpc_to_name")]
	private ImmutableDictionary<string, string> _lnpcToName = ImmutableDictionary<string, string>.Empty;

	public string? GetName(int id)
	{
		var lnpc = _idToLnpc.TryGetValue(id, out var value) ? value : null;
		if (lnpc == null) return null;
		return _lnpcToName.TryGetValue(lnpc, out var value2) ? value2.Trim() : null;
	}
}

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

// @formatter:off
/// <summary>
/// The type of a game object.
/// </summary>
public enum ObjectType
{
	None        = 0x0,
	Morph       = 0x1,
	Item        = 0x2,
	Collision   = 0x3,
	Ground      = 0x4,
	Building    = 0x5,
	Npc         = 0x6,
	Mob         = 0x7,
	Avatar      = 0x8, // external user
	User        = 0x9, // current user player
	Cart        = 0xA,
	CastleGear  = 0xB,
	EventObject = 0xC,
	Ride        = 0xD
}
// @formatter:on

public enum ItemEarningPriority
{
	FreeForAll,
	EvenShare
}

public enum ExpDivision
{
	EqualShareOn,
	EqualShareOff
}

public class Party
{
	private readonly ByteValue _inPartyField;
	private readonly IntValue _memberCountField;
	private readonly ByteValue _levelField;
	private readonly IntValue _expField;
	private readonly ByteValue _optionsField;

	public bool IsInParty => _inPartyField.GetValue() == 0x1;

	public int MemberCount => _memberCountField.GetValue();

	public int Level => _levelField.GetValue();

	public int Exp => _expField.GetValue();

	public ItemEarningPriority ItemEarningPriority => (ItemEarningPriority)(_optionsField.GetValue() & (1 << 7) >> 7);

	public ExpDivision ExpDivision => (ExpDivision)(_optionsField.GetValue() & 1);

	public PartyMember[] PartyMembers
	{
		get
		{
			if (!IsInParty) return Array.Empty<PartyMember>();
			var members = new PartyMember[MemberCount];
			for (var i = 0; i < MemberCount; i++)
			{
				members[i] = new PartyMember(i);
			}

			return members;
		}
	}

	internal Party()
	{
		_inPartyField = new ByteValue(new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x19));
		_memberCountField =
			new IntValue(new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x40));
		_levelField = new ByteValue(new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x1C));
		_expField = new IntValue(new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x20));
		_optionsField = new ByteValue(new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x18));
	}
}

public class PartyMember
{
	private TwoByteValue _serverIdField;
	private StringValue _nameField;
	
	public int ServerId => _serverIdField.GetValue();

	public int Id => GameObjects.GetClientId(ServerId);

	public string Name => _nameField.GetValue();

	public bool IsVisible => Id != 0;

	public TargetedEntity? Entity => !IsVisible ? null : new TargetedEntity(Id);

	internal PartyMember(int index)
	{
		var partyMemberListAddress = new MemoryAddress(StaticOffsets.ApplicationName, StaticOffsets.PartyBase + 0x38);
		var memberBase = new WrappedMemoryAddress(partyMemberListAddress, new int[index + 1]);
		_serverIdField = new TwoByteValue(new WrappedMemoryAddress(memberBase, 0x14));
		_nameField = new StringValue(new WrappedMemoryAddress(memberBase, 0x38));
	}
}

//public class PartyMemberList : IEnumerator

/// <summary>
/// Represents an entity in the game.
/// </summary>
public abstract class Entity
{
	private readonly TwoByteValue _idField;
	private readonly LongValue _typeInstrAddrValue;
	private readonly FloatValue _rawPosXField;
	private readonly FloatValue _rawPosYField;
	private readonly FloatValue _rawPosZField;
	private readonly FloatValue _posXField;
	private readonly FloatValue _posYField;
	private readonly FloatValue _posZField;
	private readonly TwoByteValue _currentStateField;
	private readonly TwoByteValue _currentCommandField;
	private readonly IntValue _activeObjectId;
	private readonly ByteValue _runMode;
	private readonly ByteValue _moveMode;

	public float PositionX => _rawPosXField.GetValue() / 100f;

	public float PositionY => _rawPosYField.GetValue() / 100f;

	public float PositionZ
	{
		get => _rawPosZField.GetValue() / 100f;
		set => _posZField.WriteValue(value);
	}

	public int Id => _idField.GetValue();

	public ObjectType Type
	{
		get
		{
			// In the game code, the type is a static enum value that is returned. Because of this,
			// the game compiles the enum to an instruction that is ran whenever the GetType method
			// of an object is called. This means that the entity type isn't a simple memory value 
			// that can be read and so we need to trace the instruction to get the value. The 
			// instruction set of an entity is always at offset 0x0, and the GetType instruction 
			// is at 0x40 within the instruction set.
			var instrAddress = _typeInstrAddrValue.GetValue();
			var jmp = MainWindow.TargetApplicationMemory.ReadBytes($"{instrAddress:x8}", 5);
			if (jmp == null || jmp[0] != 0xE9) // Expecting a `jmp` instruction to the `mov eax,<id>` instruction
				return ObjectType.None;
			var jmpAmt = BitConverter.ToInt32(jmp.Skip(1).Take(4).ToArray()) + 0x5; // 0x5 added for # jmp bytes

			var mov = MainWindow.TargetApplicationMemory.ReadBytes($"{instrAddress + jmpAmt:x8}", 5);
			if (mov == null || mov[0] != 0xB8) // Expecting a `mov eax,<int32>`. 0xB8 is op code for mov eax.
				return ObjectType.None;
			// we take the int32 value being assigned, this is the type enum being returned by the game
			return (ObjectType)BitConverter.ToInt32(mov.Skip(1).Take(4).ToArray());
		}
	}

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
		_typeInstrAddrValue = new LongValue(new WrappedMemoryAddress(baseAddress, 0x0, 0x40));
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
	/// Calculates the distance to an X, Y coordinate.
	/// </summary>
	/// <param name="x">other X coordinate</param>
	/// <param name="y">other Y coordinate</param>
	/// <returns>distance between this entity and the provided coordinates</returns>
	public int GetDistanceTo(float x, float y)
	{
		var diffX = Math.Abs(PositionX) - Math.Abs(x);
		var diffY = Math.Abs(PositionY) - Math.Abs(y);
		return (int)Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2));
	}

	/// <summary>
	/// Calculates the distance to another entity.
	/// </summary>
	/// <param name="entity">entity target</param>
	/// <returns>distance between this entity and the provided entity</returns>
	public int GetDistanceTo(Entity entity)
	{
		return GetDistanceTo(entity.PositionX, entity.PositionY);
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
	//private readonly IntValue _mapIdField;
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
	//public int MapId => _mapIdField.GetValue();

	public Camera Camera { get; } = new();

	public Party Party { get; } = new();

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

	internal Character(IMemoryAddress baseAddress) : base(baseAddress)
	{
		_nameField = new StringValue(new WrappedMemoryAddress(baseAddress, 0xB10));
		_levelField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x3AD8));
		_xpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3AF0));
		_zulyField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3D58));
		_hpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3AE8));
		_mpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x3AEC));
		_maxHpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x4620));
		_maxMpField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x4624));
		//_mapIdField = new IntValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentMapId));
		_targetIdField = new IntValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentTarget, 0x08));
		_targetNameField = new StringValue(new MemoryAddress("trose.exe", StaticOffsets.CurrentTargetName));
		_consumedSummonsMeterField = new IntValue(new WrappedMemoryAddress(baseAddress, 0x57C0));
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
	private readonly TwoByteValue _idKeyField;

	private readonly int _originalId;

	public int Hp => _hpField.GetValue();
	public int MaxHp => _maxHpField.GetValue();
	public int Mp => _mpField.GetValue();
	public int MaxMp => _maxMpField.GetValue();

	public string? Name => Type is ObjectType.Npc or ObjectType.Mob ? GameObjects.GetNpcName(Key) : null;
	public int Key => _idKeyField.GetValue();

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
		_idKeyField = new TwoByteValue(new WrappedMemoryAddress(baseAddress, 0x2C0));
	}

	public override bool IsValid()
	{
		return Id == _originalId && MaxHp > 0;
	}

	internal static int _createBaseOffset(int id)
	{
		return (id * 8) + 0x22078;
	}
}

public class TargetedCharacter : TargetedEntity
{
	private readonly StringValue _nameField;
	public string Name => _nameField.GetValue();

	public TargetedCharacter(int id) : base(id)
	{
		var baseAddress = new MemoryAddress(StaticOffsets.ApplicationName,
			StaticOffsets.EntityList, _createBaseOffset(id));
		_nameField = new StringValue(new WrappedMemoryAddress(baseAddress, 0xB10));
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
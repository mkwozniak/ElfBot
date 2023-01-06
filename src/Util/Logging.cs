using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElfBot;

public sealed class Logger
{
	private readonly Queue<LogEntry> _entries = new();
	public ReadOnlyCollection<LogEntry> Entries => Array.AsReadOnly<LogEntry>(_entries.ToArray());

	private readonly uint _bufferSize;
	private readonly object _lock = new object();

	public Logger(uint bufferSize = 1000)
	{
		_bufferSize = bufferSize;
	}

	public void Clear()
	{
		_entries.Clear();
	}

	public void Log(Level level, String text)
	{
		Log(level, text, Array.Empty<LogEntryTag>());
	}

	public void Log(Level level, String text, params LogEntryTag[] tags)
	{
		lock (_lock)
		{
			var entry = new LogEntry(DateTime.Now, level, text, new ReadOnlyCollection<LogEntryTag>(tags));
			_entries.Enqueue(entry);
			if (Entries.Count > _bufferSize)
			{
				_entries.Dequeue();
			}
		}
	}

	public void Debug(String text)
	{
		Debug(text, Array.Empty<LogEntryTag>());
	}

	public void Debug(String text, params LogEntryTag[] tags)
	{
		Log(Level.Debug, text, tags);
	}

	public void Info(String text)
	{
		Info(text, Array.Empty<LogEntryTag>());
	}

	public void Info(String text, params LogEntryTag[] tags)
	{
		Log(Level.Info, text, tags);
	}

	public void Warn(String text)
	{
		Warn(text, Array.Empty<LogEntryTag>());
	}

	public void Warn(String text, params LogEntryTag[] tags)
	{
		Log(Level.Warn, text, tags);
	}

	public void Error(String text)
	{
		Error(text, Array.Empty<LogEntryTag>());
	}

	public void Error(String text, params LogEntryTag[] tags)
	{
		Log(Level.Error, text, tags);
	}
}

public class LogEntry
{
	public readonly DateTime TimeStamp;
	public readonly Level Level;
	public readonly string Text;
	public readonly ReadOnlyCollection<LogEntryTag> Tags;

	public LogEntry(DateTime ts, Level level, string text, ReadOnlyCollection<LogEntryTag> tags)
	{
		TimeStamp = ts;
		Level = level;
		Text = text;
		Tags = tags;
	}
}

public class LogEntryTag : Tuple<string, string>
{
	public static readonly LogEntryTag Food = new LogEntryTag("type", "Food");
	public static readonly LogEntryTag Combat = new LogEntryTag("type", "Combat");
	public static readonly LogEntryTag System = new LogEntryTag("type", "System");
	public static readonly LogEntryTag Camera = new LogEntryTag("type", "Camera");

	public LogEntryTag(string tag, string value) : base(tag, value)
	{
	}
}

/// <summary>
/// Logging level options.
/// </summary>
public enum Level
{
	Debug = 10,
	Info = 20,
	Warn = 30,
	Error = 40
}
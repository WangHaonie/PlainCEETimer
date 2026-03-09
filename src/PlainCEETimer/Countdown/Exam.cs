using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Exam : IListViewData<Exam>
{
    public string Name
    {
        get;
        init
        {
            value = value.RemoveIllegalChars();

            if (ConfigValidator.ValidateNeeded && !ConfigValidator.IsValidExamLength(value.Length))
            {
                throw ConfigValidator.InvalidTampering(ConfigField.ExamNameLength);
            }

            field = value;
        }
    } = "";

    [JsonConverter(typeof(ExamTimeConverter))]
    public DateTime Start
    {
        get;
        init
        {
            if (ConfigValidator.ValidateNeeded)
            {
                ConfigValidator.EnsureExamDate(value);
            }

            field = value.TruncateToSeconds();
        }
    } = DateTime.Now;

    [JsonConverter(typeof(ExamTimeConverter))]
    public DateTime End
    {
        get;
        init
        {
            if (ConfigValidator.ValidateNeeded)
            {
                ConfigValidator.EnsureExamDate(value);
            }

            field = value.TruncateToSeconds();
        }
    } = DateTime.Now;

    public ExamSettings Settings { get; init; }

    public bool Excluded { get; set; }

    public int CompareTo(Exam other)
    {
        if (other == null)
        {
            return 1;
        }

        int order = Start.CompareTo(other.Start);

        if (order != 0)
        {
            return order;
        }

        if ((order = End.CompareTo(other.End)) != 0)
        {
            return order;
        }

        return string.CompareOrdinal(Name, other.Name);
    }

    public bool Equals(Exam other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Start == other.Start
            && End == other.End
            && Name == other.Name
            && Settings.Equals(other.Settings);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Exam);
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(Name)
            .Add(Start)
            .Add(End)
            .Combine();
    }

    public object Clone()
    {
        return new Exam()
        {
            Name = Name,
            Start = Start,
            End = End,
            Settings = Settings,
            Excluded = Excluded
        };
    }

    bool IListViewData<Exam>.Default { get; init; }

    bool IListViewData<Exam>.InternalEquals(Exam other)
    {
        return Equals(other);
    }

    private string DebuggerDisplay => $"{Name}: {Start.Format()} ~ {End.Format()}";
}
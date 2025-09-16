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
        set
        {
            value = value.RemoveIllegalChars();

            if (Validator.ValidateNeeded && !Validator.IsValidExamLength(value.Length))
            {
                throw Validator.InvalidTampering(ConfigField.ExamNameLength);
            }

            field = value;
        }
    } = "";

    [JsonConverter(typeof(ExamTimeConverter))]
    public DateTime Start
    {
        get;
        set
        {
            if (Validator.ValidateNeeded)
            {
                Validator.EnsureExamDate(value);
            }

            field = value.TruncateToSeconds();
        }
    } = DateTime.Now;

    [JsonConverter(typeof(ExamTimeConverter))]
    public DateTime End
    {
        get;
        set
        {
            if (Validator.ValidateNeeded)
            {
                Validator.EnsureExamDate(value);
            }

            field = value.TruncateToSeconds();
        }
    } = DateTime.Now;

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
        return other != null
            && Start == other.Start
            && End == other.End
            && Name == other.Name;
    }

    public override string ToString()
    {
        return $"{Name.Truncate(6)} ({Start.Format()})";
    }

    public override bool Equals(object obj)
    {
        return Equals((Exam)obj);
    }

    public override int GetHashCode()
    {
        return unchecked((17 * 23 + Name.GetHashCode()) * 23 + Start.GetHashCode() + End.GetHashCode());
    }

    bool IListViewData<Exam>.InternalEquals(Exam other)
    {
        return Equals(other);
    }

    private string DebuggerDisplay => $"{Name}: {Start.Format()} ~ {End.Format()}";
}
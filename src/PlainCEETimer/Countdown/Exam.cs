using System;
using System.Collections.Generic;
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
    private static readonly ExamNormalEqualityComparer NormalComparer = new();
    private static readonly ExamFullEqualityComparer FullComparer = new();

    private class ExamNormalEqualityComparer : IEqualityComparer<Exam>
    {
        public bool Equals(Exam x, Exam y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Start == y.Start
                && x.End == y.End
                && x.Name == y.Name;
        }

        public int GetHashCode(Exam obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return new HashCode()
                .Add(obj.Start)
                .Add(obj.End)
                .Add(obj.NameHashCode)
                .Combine();
        }
    }

    private class ExamFullEqualityComparer : IEqualityComparer<Exam>
    {
        public bool Equals(Exam x, Exam y)
        {
            if (x.Equals(x))
            {
                return true;
            }

            var s1 = x.Settings;
            var s2 = y.Settings;

            if (s1 == null || s2 == null)
            {
                return false;
            }

            if (ReferenceEquals(s1, s2))
            {
                return true;
            }

            return s1.Equals(s2);
        }

        public int GetHashCode(Exam obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return new HashCode()
                .Add(obj)
                .Add(obj.Settings)
                .Combine();
        }
    }

    public string Name
    {
        get;
        init
        {
            value = value.Clean();

            if (ConfigValidator.ValidateNeeded && !ConfigValidator.IsValidExamLength(value.Length))
            {
                throw ConfigValidator.InvalidTampering(ConfigField.ExamNameLength);
            }

            NameHashCode = new(value);
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

    private readonly StringHashCodeProvider NameHashCode;

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
        return NormalComparer.Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Exam);
    }

    public override int GetHashCode()
    {
        return NormalComparer.GetHashCode(this);
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
        return FullComparer.Equals(this, other);
    }

    private string DebuggerDisplay => $"{Name}: {Start.Format()} ~ {End.Format()}";
}
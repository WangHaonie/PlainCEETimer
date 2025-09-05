using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ExamInfoObject : IListViewData<ExamInfoObject>
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

    public int CompareTo(ExamInfoObject other)
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

    public bool Equals(ExamInfoObject other)
    {
        /*

        DateTime 比较时只精确到秒 参考：

        C# DateTime 精确到秒/截断毫秒部分 - eshizhan - 博客园
        https://www.cnblogs.com/eshizhan/archive/2011/11/15/2250007.html

        */

        return other != null
            && Start == other.Start
            && End == other.End
            && Name == other.Name;
    }

    public override string ToString()
    {
        return $"{Name.Truncate(6)} - {Start.Format()}";
    }

    public override bool Equals(object obj)
    {
        return Equals((ExamInfoObject)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (17 * 23 + Name.GetHashCode()) * 23 + Start.GetHashCode() + End.GetHashCode();
        }
    }

    bool IListViewData<ExamInfoObject>.InternalEquals(ExamInfoObject other)
    {
        return Equals(other);
    }

    private string DebuggerDisplay => $"{Name}: [{Start.Format()}]~[{End.Format()}]";
}
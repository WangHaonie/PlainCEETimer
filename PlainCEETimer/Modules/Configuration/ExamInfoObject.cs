using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    [DebuggerDisplay("{Name,nq}: {Start.Format(),nq}~{End.Format(),nq}")]
    public class ExamInfoObject : IListViewData<ExamInfoObject>
    {
        public string Name
        {
            get;
            set
            {
                value = value.RemoveIllegalChars();

                if (MainForm.ValidateNeeded && !Validator.IsValidExamLength(value.Length))
                {
                    throw new Exception();
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
                if (MainForm.ValidateNeeded)
                {
                    Validator.EnsureExamDate(value);
                }

                field = value;
                StartInternalTicks = value.Ticks / TimeSpan.TicksPerSecond;
            }
        } = DateTime.Now;

        [JsonConverter(typeof(ExamTimeConverter))]
        public DateTime End
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    Validator.EnsureExamDate(value);
                }

                field = value;
                EndInternalTicks = value.Ticks / TimeSpan.TicksPerSecond;
            }
        } = DateTime.Now;

        private long StartInternalTicks;
        private long EndInternalTicks;

        public int CompareTo(ExamInfoObject other)
        {
            if (other == null)
            {
                return 1;
            }

            int order = StartInternalTicks.CompareTo(other.StartInternalTicks);

            if (order != 0)
            {
                return order;
            }

            if ((order = EndInternalTicks.CompareTo(other.EndInternalTicks)) != 0)
            {
                return order;
            }

            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public bool Equals(ExamInfoObject other)
        {
            /*
            
            DateTime 比较时只精确到秒 参考：

            C# DateTime 精确到秒/截断毫秒部分 - eshizhan - 博客园
            https://www.cnblogs.com/eshizhan/archive/2011/11/15/2250007.html

            */

            return other != null
                && Name == other.Name
                && StartInternalTicks == other.StartInternalTicks
                && EndInternalTicks == other.EndInternalTicks;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name.Truncate(6), Start.Format());
        }

        public override bool Equals(object obj)
        {
            return Equals((ExamInfoObject)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (17 * 23 + Name.GetHashCode()) * 23 + StartInternalTicks.GetHashCode() + EndInternalTicks.GetHashCode();
            }
        }

        bool IListViewData<ExamInfoObject>.InternalEquals(ExamInfoObject other)
        {
            return Equals(other);
        }
    }
}

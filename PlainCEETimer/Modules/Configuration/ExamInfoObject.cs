using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Forms;
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
            }
        } = DateTime.Now;

        public int CompareTo(ExamInfoObject other)
        {
            return other == null ? 1 : Start.CompareTo(other.Start);
        }

        public bool Equals(ExamInfoObject other)
        {
            if (other == null)
            {
                return false;
            }

            /*
            
            DateTime 比较时只精确到秒 参考：

            C# DateTime 精确到秒/截断毫秒部分 - eshizhan - 博客园
            https://www.cnblogs.com/eshizhan/archive/2011/11/15/2250007.html

            */

            return Name == other.Name && Start.Ticks / TimeSpan.TicksPerSecond == other.Start.Ticks / TimeSpan.TicksPerSecond;
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
                return (17 * 23 + Name.GetHashCode()) * 23 + Start.GetHashCode();
            }
        }
    }
}

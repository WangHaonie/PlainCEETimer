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

            return Name == other.Name && Start == other.Start;
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

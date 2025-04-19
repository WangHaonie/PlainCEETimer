using System;
using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ExamInfoObject : IListViewObject<ExamInfoObject>
    {
        public string Name
        {
            get;
            set
            {
                value = value.RemoveIllegalChars();

                if (MainForm.ValidateNeeded)
                {
                    if (!Validator.IsValidExamLength(value.Length))
                    {
                        throw new Exception();
                    }
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
                if (MainForm.ValidateNeeded && !Validator.IsValidExamDate(value))
                {
                    throw new Exception();
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
                if (MainForm.ValidateNeeded && !Validator.IsValidExamDate(value))
                {
                    throw new Exception();
                }

                field = value;
            }
        } = DateTime.Now;

        public bool CanExecute() => true;

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
            return string.Format("{0} - {1}", Name.Truncate(6), Start.ToFormatted());
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

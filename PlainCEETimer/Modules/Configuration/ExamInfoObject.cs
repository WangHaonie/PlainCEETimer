using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using System;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ExamInfoObject : IListViewObject<ExamInfoObject>
    {
        public string Name
        {
            get;
            set
            {
                if (MainForm.ValidateNeeded)
                {
                    if (!value.Length.IsValid())
                    {
                        throw new Exception();
                    }
                }

                field = value.RemoveIllegalChars();
            }
        } = "";

        [JsonConverter(typeof(ExamTimeConverter))]
        public DateTime Start { get; set; } = DateTime.Now;

        [JsonConverter(typeof(ExamTimeConverter))]
        public DateTime End { get; set; } = DateTime.Now;

        public bool CanExecute() => true;

        public int CompareTo(ExamInfoObject other)
            => other == null ? 1 : Start.CompareTo(other.Start);

        public bool Equals(ExamInfoObject other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name && Start == other.Start;
        }

        public override string ToString()
            => string.Format("{0} - {1}", Name, Start.ToString(App.DateTimeFormat));

        public override bool Equals(object obj)
            => Equals((ExamInfoObject)obj);

        public override int GetHashCode()
        {
            unchecked
            {
                return (17 * 23 + Name.GetHashCode()) * 23 + Start.GetHashCode();
            }
        }
    }
}

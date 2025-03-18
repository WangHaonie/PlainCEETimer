using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using System;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ExamInfoObject : IComparable<ExamInfoObject>, IEquatable<ExamInfoObject>
    {
        public string Name
        {
            get => field;
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
    }
}

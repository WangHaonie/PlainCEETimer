using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using System;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ExamInfoObject : IComparable<ExamInfoObject>
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

        public override string ToString()
            => string.Format("{0} - {1}", Name, Start.ToString(App.DateTimeFormat));

        int IComparable<ExamInfoObject>.CompareTo(ExamInfoObject other)
        {
            return other == null ? 1 : Start.CompareTo(other.Start);
        }
    }
}

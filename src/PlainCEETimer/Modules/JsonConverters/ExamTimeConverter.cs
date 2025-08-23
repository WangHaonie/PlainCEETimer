using Newtonsoft.Json.Converters;

namespace PlainCEETimer.Modules.JsonConverters
{
    public sealed class ExamTimeConverter : IsoDateTimeConverter
    {
        public ExamTimeConverter()
        {
            DateTimeFormat = App.DateTimeFormat;
        }
    }
}

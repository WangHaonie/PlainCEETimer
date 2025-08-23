using System.ComponentModel;
using System.Runtime.Serialization;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration
{
    public class DisplayObject
    {
        public bool ShowXOnly { get; set; }

        public int X
        {
            get;
            set => Validator.SetValue(ref field, value, 6, 0);
        }

        [DefaultValue(2)]
        public int EndIndex
        {
            get;
            set => Validator.SetValue(ref field, value, 2, 0, 2);
        } = 2;

        public bool CustomText { get; set; }

        public int ScreenIndex { get; set; }

        [DefaultValue(CountdownPosition.TopCenter)]
        public CountdownPosition Position { get; set; } = CountdownPosition.TopCenter;

        public bool Draggable { get; set; }

        public bool SeewoPptsvc { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            CustomText = Validator.ValidateBoolean(CustomText, !ShowXOnly);
        }
    }
}

using System.ComponentModel;
using System.Runtime.Serialization;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration
{
    public class DisplayObject
    {
        public bool ShowXOnly
        {
            get;
            set
            {
                field = value;
            }
        }

        public int X
        {
            get;
            set => ConfigObject.SetValue(ref field, value, 6, 0);
        }

        [DefaultValue(2)]
        public int EndIndex
        {
            get;
            set => ConfigObject.SetValue(ref field, value, 2, 0, 2);
        } = 2;

        public bool CustomText
        {
            get;
            set
            {
                field = value;
            }
        }

        public int ScreenIndex { get; set; }

        [DefaultValue(CountdownPosition.TopCenter)]
        public CountdownPosition Position { get; set; } = CountdownPosition.TopCenter;

        public bool Draggable { get; set; }

        public bool SeewoPptsvc { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            CustomText = ConfigObject.InvalidateBoolean(CustomText, !ShowXOnly);
        }
    }
}

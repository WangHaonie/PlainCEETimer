using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    public class GeneralObject
    {
        public bool AutoSwitch { get; set; }

        public int Interval { get; set; }

        [DefaultValue(true)]
        public bool TrayIcon { get; set; } = true;

        public bool TrayText { get; set; }

        public bool MemClean { get; set; }

        [DefaultValue(true)]
        public bool TopMost { get; set; } = true;

        [DefaultValue(true)]
        public bool UniTopMost { get; set; } = true;

        [DefaultValue(Validator.MaxOpacity)]
        public int Opacity
        {
            get;
            set => Validator.SetValue(ref field, value, Validator.MaxOpacity, Validator.MinOpacity, Validator.MaxOpacity);
        } = Validator.MaxOpacity;

        public bool CustomColor { get; set; }

        [JsonConverter(typeof(ColorFormatConverter))]
        public Color BorderColor { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            TrayText = Validator.ValidateBoolean(TrayText, TrayIcon);
        }
    }
}

using System.ComponentModel;
using System.Drawing;
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
            set => field = (value is < Validator.MinOpacity or > Validator.MaxOpacity) ? Validator.MaxOpacity : value;
        } = Validator.MaxOpacity;

        public bool CustomColor { get; set; }

        [JsonConverter(typeof(ColorFormatConverter))]
        public Color BorderColor { get; set; }
    }
}

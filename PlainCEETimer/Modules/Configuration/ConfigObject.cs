using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PlainCEETimer.Modules.Configuration
{
    public sealed class ConfigObject
    {
        public GeneralObject General { get; set; } = new();

        public DisplayObject Display { get; set; } = new();

        public AppearanceObject Appearance { get; set; } = new();

        public ToolsObject Tools { get; set; } = new();

        public CustomRuleObject[] CustomRules
        {
            get;
            set
            {
                if (value != null)
                {
                    if (MainForm.ValidateNeeded)
                    {
                        var HashSet = new HashSet<CustomRuleObject>();

                        foreach (var Item in value)
                        {
                            if (!HashSet.Add(Item))
                            {
                                throw new Exception();
                            }
                        }

                        Array.Sort(value);
                    }

                    field = value;
                }
            }
        } = [];

        [JsonConverter(typeof(CustomColorsConverter))]
        public int[] CustomColors { get; set; } = DefaultValues.ColorDialogColors;

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Pos { get; set; }
    }
}

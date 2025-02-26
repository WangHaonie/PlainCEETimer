using Newtonsoft.Json;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules.JsonConverters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
            get => field ?? [];
            set
            {
                if (value == null)
                {
                    field = [];
                }
                else
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
        }

        public int[] CustomColors { get; set; } = [.. Enumerable.Repeat(16777215, 16)];

        [JsonConverter(typeof(PointFormatConverter))]
        public Point Pos { get; set; }
    }
}

using System;
using System.Drawing;

namespace PlainCEETimer.UI;

public class DipFont(Font font) : IEquatable<DipFont>
{
    public Font Value => font;

    public float Size => font.Size;

    public bool Equals(DipFont other)
    {
        return other.Value.Equals(Value);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as DipFont);
    }

    public override int GetHashCode()
    {
        return font.GetHashCode();
    }
}
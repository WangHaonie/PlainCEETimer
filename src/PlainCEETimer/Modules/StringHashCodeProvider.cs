namespace PlainCEETimer.Modules;

public class StringHashCodeProvider(string str)
{
    private int? hashCode;

    public override int GetHashCode()
    {
        return hashCode ??= str.GetHashCode();
    }
}
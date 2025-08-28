namespace PlainCEETimer.Interop.Extensions;

public static class Extensions
{
    extension(ushort value)
    {
        public byte LoByte => (byte)(value & 0xFF);
        public byte HiByte => (byte)(value >> 8);

        public static ushort MakeWord(byte low, byte high)
        {
            return (ushort)(low | (high << 8));
        }
    }

    extension(int value)
    {
        public ushort LoWord => (ushort)(value & 0xFFFF);
        public ushort HiWord => (ushort)(value >> 16);

        public static int MakeLong(ushort low, ushort high)
        {
            return low | (high << 16);
        }
    }
}

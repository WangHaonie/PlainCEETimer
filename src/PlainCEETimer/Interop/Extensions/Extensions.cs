namespace PlainCEETimer.Interop.Extensions;

public static class Extensions
{
    extension(ushort value)
    {
        public byte LoByte => (byte)(value & 0xFF);
        public byte HiByte => (byte)((value >> 8) & 0xFF);

        public static ushort MakeWord(byte low, byte high)
        {
            return (ushort)(low | (high << 8));
        }
    }

    extension(int value)
    {
        public int LoWord => value & 0xFFFF;
        public int HiWord => (value >> 16) & 0xFFFF;

        public static int MakeLong(int low, int high)
        {
            return (ushort)low | ((ushort)high << 16);
        }
    }
}

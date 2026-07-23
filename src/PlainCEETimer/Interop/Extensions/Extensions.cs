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

        public static int MakeLong24(int low, int high)
        {
            return (int)(((uint)low & 0xFFFFFF) | ((uint)high << 24));
        }
    }

    //extension(long value)
    //{
    //    public int LoDWord => (int)(uint)(value & 0xFFFFFFFF);
    //    public int HiDWord => (int)(uint)((value >> 32) & 0xFFFFFFFF);

    //    public static long MakeQWord(int low, int high)
    //    {
    //        return (long)(((uint)low) | ((ulong)(uint)high << 32));
    //    }
    //}
}

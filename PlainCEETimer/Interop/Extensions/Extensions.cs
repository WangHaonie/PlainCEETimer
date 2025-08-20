namespace PlainCEETimer.Interop.Extensions
{
    public static class Extensions
    {
        extension(ushort value)
        {
            public byte LoByte => (byte)(value & 0xFF);
            public byte HiByte => (byte)(value >> 8);

            public static ushort FromBytes(byte low, byte high)
            {
                return (ushort)(low | (ushort)(high << 8));
            }
        }
    }
}

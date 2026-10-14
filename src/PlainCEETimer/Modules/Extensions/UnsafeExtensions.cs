using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Extensions;

public unsafe static class UnsafeExtensions
{
    extension(Unsafe)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(nint ptr, int offset)
        {
            return Unsafe.ReadUnaligned<int>((byte*)ptr + offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(nint ptr, int offset, int value)
        {
            Unsafe.WriteUnaligned((byte*)ptr + offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint ReadIntPtr(nint ptr, int offset)
        {
            return Unsafe.ReadUnaligned<nint>((byte*)ptr + offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteIntPtr(nint ptr, int offset, nint value)
        {
            Unsafe.WriteUnaligned((byte*)ptr + offset, value);
        }
    }
}

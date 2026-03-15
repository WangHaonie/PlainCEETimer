using Mono.Cecil;

namespace PlainCEETimer.Fody;

internal static class Extensions
{
    extension(TypeDefinition value)
    {
        public bool IsStruct => value.IsValueType && !value.IsEnum;
    }
}
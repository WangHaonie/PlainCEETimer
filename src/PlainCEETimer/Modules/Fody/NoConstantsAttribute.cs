using System;

namespace PlainCEETimer.Modules.Fody;

/// <summary>
/// 在编译时移除该对象中声明的常量。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class NoConstantsAttribute : Attribute;
using System;

namespace PlainCEETimer.Modules.Annotations.Fody;

/// <summary>
/// 在编译时移除该对象中声明的常量字段。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class NoConstantsAttribute : Attribute;
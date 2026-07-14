using System;

namespace PlainCEETimer.Modules.Annotations.Fody;

/// <summary>
/// 在编译时从程序集中删除该对象。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class CompilerRemoveAttribute : Attribute;
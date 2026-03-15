using System;

namespace PlainCEETimer.Modules.Fody;

/// <summary>
/// 在编译时删除该对象的声明。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CompilerRemoveAttribute : Attribute;
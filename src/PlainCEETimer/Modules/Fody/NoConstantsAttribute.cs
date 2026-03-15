using System;

namespace PlainCEETimer.Modules.Fody;

/// <summary>
/// 在编译程序集时移除该类中声明的所有常量。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NoConstantsAttribute : Attribute;
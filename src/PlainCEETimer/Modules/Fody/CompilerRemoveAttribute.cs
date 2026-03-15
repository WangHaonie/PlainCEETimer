using System;

namespace PlainCEETimer.Modules.Fody;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CompilerRemoveAttribute : Attribute;
using System;

namespace PlainCEETimer.Modules.Fody;

/// <summary>
/// 用于排除将被 <see cref="NoConstantsAttribute"/> 移除的常量
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ConstantAttribute : Attribute;
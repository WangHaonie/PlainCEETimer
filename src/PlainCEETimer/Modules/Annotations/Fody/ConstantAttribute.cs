using System;

namespace PlainCEETimer.Modules.Annotations.Fody;

/// <summary>
/// 保留将被 <see cref="NoConstantsAttribute"/> 移除的常量字段。
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal sealed class ConstantAttribute : Attribute;
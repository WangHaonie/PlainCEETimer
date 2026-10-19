using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Reflection;

public static class ReflectionExtensions
{
    public static FieldInfo RevealField(this Type t, [CallerMemberName] string name = default)
    {
        return t.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static PropertyInfo RevealProperty(this Type t, [CallerMemberName] string name = default)
    {
        return t.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    public static Func<TObject, TField> CreateGetter<TObject, TField>(this FieldInfo fi)
    {
        if (fi != null)
        {
            var instance = Expression.Parameter(typeof(TObject));
            var field = Expression.Field(instance, fi);
            return Expression.Lambda<Func<TObject, TField>>(field, instance).Compile();
        }

        return null;
    }

    public static Action<TObject, TField> CreateSetter<TObject, TField>(this FieldInfo fi)
    {
        if (fi != null)
        {
            var instance = Expression.Parameter(typeof(TObject));
            var value = Expression.Parameter(typeof(TField));
            var field = Expression.Field(instance, fi);
            var assign = Expression.Assign(field, value);
            return Expression.Lambda<Action<TObject, TField>>(assign, instance, value).Compile();
        }

        return null;
    }

    public static Func<TObject, TProperty> CreateGetter<TObject, TProperty>(this PropertyInfo pi)
    {
        if (pi != null && pi.CanRead)
        {
            var instance = Expression.Parameter(typeof(TObject));
            var prop = Expression.Property(instance, pi);
            return Expression.Lambda<Func<TObject, TProperty>>(prop, instance).Compile();
        }

        return null;
    }

    public static Action<TObject, TProperty> CreateSetter<TObject, TProperty>(this PropertyInfo pi)
    {
        if (pi != null && pi.CanWrite)
        {
            var instance = Expression.Parameter(typeof(TObject));
            var value = Expression.Parameter(typeof(TProperty));
            var prop = Expression.Property(instance, pi);
            var assign = Expression.Assign(prop, value);
            return Expression.Lambda<Action<TObject, TProperty>>(assign, instance, value).Compile();
        }

        return null;
    }
}

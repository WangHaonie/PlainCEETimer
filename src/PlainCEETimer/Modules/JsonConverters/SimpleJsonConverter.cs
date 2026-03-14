using System;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.JsonConverters;

/// <summary>
/// 在一个复杂类型与单个简单类型之间进行转换
/// </summary>
public abstract class SimpleJsonConverter<TObject, TValue> : JsonConverter<TObject>
{
    public sealed override TObject ReadJson(JsonReader reader, Type objectType, TObject existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return Deserialize(serializer.Deserialize<TValue>(reader));
    }

    public sealed override void WriteJson(JsonWriter writer, TObject value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, Serialize(value));
    }

    protected abstract TObject Deserialize(TValue value);

    protected abstract TValue Serialize(TObject obj);
}
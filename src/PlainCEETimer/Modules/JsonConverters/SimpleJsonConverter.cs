using System;
using Newtonsoft.Json;

namespace PlainCEETimer.Modules.JsonConverters;

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
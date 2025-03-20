using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LanguageExt.JsonSerializer
{
    /// <summary>
    /// JsonConverter for Option<T> that serializes None as empty array and Some as single-element array
    /// </summary>
    public class OptionArrayConverter<T> : JsonConverter<Option<T>>
    {
        private readonly JsonConverter<T> _valueConverter;
        private readonly Type _valueType;

        public OptionArrayConverter(JsonSerializerOptions options)
        {
            // Get the converter for T
            _valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));
            _valueType = typeof(T);
        }

        public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return None;
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"Expected start of array when deserializing Option<{_valueType.Name}>, but got {reader.TokenType}");
            }

            // Read the first token after the start of array
            reader.Read();

            // If we immediately hit the end of the array, it's None
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return None;
            }

            // Read the value
            T value = _valueConverter.Read(ref reader, _valueType, options);

            // Read the next token which should be end of array
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new JsonException($"Expected single item array for Option<{_valueType.Name}> but found multiple items");
            }

            return Some(value);
        }

        public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value.IsSome)
            {
                value.IfSome(v => _valueConverter.Write(writer, v, options));
            }

            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// JsonConverterFactory for creating OptionArrayConverter instances
    /// </summary>
    public class OptionArrayConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            return typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type valueType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(OptionArrayConverter<>).MakeGenericType(valueType);
            
            return (JsonConverter)Activator.CreateInstance(converterType, options);
        }
    }
}

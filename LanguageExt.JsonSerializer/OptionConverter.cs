using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LanguageExt.JsonSerializer
{
    public class OptionConverter<T> : JsonConverter<Option<T>>
    {
        private readonly JsonConverter<T> _valueConverter;
        private readonly Type _valueType;

        public OptionConverter(JsonSerializerOptions options)
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

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Try to read a wrapped Option object
                reader.Read();
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName == "isSome" && reader.TokenType == JsonTokenType.True)
                    {
                        reader.Read(); // Move to the "value" property name
                        if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "value")
                        {
                            reader.Read(); // Move to the value
                            T value = _valueConverter.Read(ref reader, _valueType, options);
                            reader.Read(); // Move past the value
                            reader.Read(); // Move past the end object
                            return Some(value);
                        }
                    }
                    else if (propertyName == "isSome" && reader.TokenType == JsonTokenType.False)
                    {
                        // Skip any remaining properties
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) { }
                        return None;
                    }
                }
                throw new JsonException("Invalid Option<T> format");
            }
            
            // Direct value (assume Some)
            T directValue = _valueConverter.Read(ref reader, _valueType, options);
            return Some(directValue);
        }

        public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
        {
            if (value.IsNone)
            {
                writer.WriteStartObject();
                writer.WriteBoolean("isSome", false);
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();
            writer.WriteBoolean("isSome", true);
            writer.WritePropertyName("value");
            
            value.IfSome(v => 
            {
                _valueConverter.Write(writer, v, options);
            });
            
            writer.WriteEndObject();
        }
    }

    public class OptionConverterFactory : JsonConverterFactory
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
            Type converterType = typeof(OptionConverter<>).MakeGenericType(valueType);
            
            return (JsonConverter)Activator.CreateInstance(converterType, options);
        }
    }
}

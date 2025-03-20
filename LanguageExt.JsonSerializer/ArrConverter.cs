using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LanguageExt.JsonSerializer
{
    public class ArrConverter<T> : JsonConverter<Arr<T>>
    {
        private readonly JsonConverter<T> _elementConverter;
        private readonly Type _elementType;

        public ArrConverter(JsonSerializerOptions options)
        {
            _elementConverter = (JsonConverter<T>)options.GetConverter(typeof(T));
            _elementType = typeof(T);
        }

        public override Arr<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected StartArray token");
            }

            var items = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                T element = _elementConverter.Read(ref reader, _elementType, options);
                items.Add(element);
            }

            return Arr.create(items.ToArray());
        }

        public override void Write(Utf8JsonWriter writer, Arr<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                _elementConverter.Write(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }

    public class ArrConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            return typeToConvert.GetGenericTypeDefinition() == typeof(Arr<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type elementType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(ArrConverter<>).MakeGenericType(elementType);
            
            return (JsonConverter)Activator.CreateInstance(converterType, options);
        }
    }
}

using LanguageExt;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LanguageExt.JsonSerializer
{
    public class EitherConverter<L, R> : JsonConverter<Either<L, R>>
    {
        private readonly JsonConverter<L> _leftConverter;
        private readonly JsonConverter<R> _rightConverter;
        private readonly Type _leftType;
        private readonly Type _rightType;

        public EitherConverter(JsonSerializerOptions options)
        {
            _leftConverter = (JsonConverter<L>)options.GetConverter(typeof(L));
            _rightConverter = (JsonConverter<R>)options.GetConverter(typeof(R));
            _leftType = typeof(L);
            _rightType = typeof(R);
        }

        public override Either<L, R> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected StartObject token");
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            string propertyName = reader.GetString();
            reader.Read();

            if (propertyName == "isRight")
            {
                bool isRight = reader.GetBoolean();
                reader.Read(); // Property name for value
                reader.Read(); // Value

                if (isRight)
                {
                    R rightValue = _rightConverter.Read(ref reader, _rightType, options);
                    // Skip to end of object
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) { }
                    return rightValue;
                }
                else
                {
                    L leftValue = _leftConverter.Read(ref reader, _leftType, options);
                    // Skip to end of object
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) { }
                    return leftValue;
                }
            }

            throw new JsonException("Invalid Either<L, R> format");
        }

        public override void Write(Utf8JsonWriter writer, Either<L, R> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.IsRight)
            {
                writer.WriteBoolean("isRight", true);
                writer.WritePropertyName("value");
                value.IfRight(right => _rightConverter.Write(writer, right, options));
            }
            else
            {
                writer.WriteBoolean("isRight", false);
                writer.WritePropertyName("value");
                value.IfLeft(left => _leftConverter.Write(writer, left, options));
            }

            writer.WriteEndObject();
        }
    }

    public class EitherConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            return typeToConvert.GetGenericTypeDefinition() == typeof(Either<,>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type[] genericArgs = typeToConvert.GetGenericArguments();
            Type converterType = typeof(EitherConverter<,>).MakeGenericType(genericArgs);
            
            return (JsonConverter)Activator.CreateInstance(converterType, options);
        }
    }
}

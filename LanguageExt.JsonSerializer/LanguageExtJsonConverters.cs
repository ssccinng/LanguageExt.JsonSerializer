using System.Text.Json;

namespace LanguageExt.JsonSerializer
{
    public static class LanguageExtJsonConverters
    {
        /// <summary>
        /// Adds all LanguageExt JSON converters to the options
        /// </summary>
        public static JsonSerializerOptions AddLanguageExtConverters(this JsonSerializerOptions options)
        {
            options.Converters.Add(new OptionConverterFactory());
            options.Converters.Add(new EitherConverterFactory());
            options.Converters.Add(new ArrConverterFactory());
            options.Converters.Add(new LstConverterFactory());
            return options;
        }
    }
}

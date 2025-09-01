namespace MyWheelApp.Models
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public class Recipe
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
        public List<string> Instructions { get; set; } = new();
        public string PrepTime { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string TotalTime { get; set; } = string.Empty;

        [JsonConverter(typeof(NumberToStringConverter))]
        public string Servings { get; set; } = string.Empty;

        public string Difficulty { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    // Converter that accepts either a JSON number or string and returns it as C# string
    public class NumberToStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? string.Empty;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                // Convert numeric token to string using invariant culture
                if (reader.TryGetInt64(out var longValue))
                {
                    return longValue.ToString(CultureInfo.InvariantCulture);
                }

                // Fallback to double
                return reader.GetDouble().ToString(CultureInfo.InvariantCulture);
            }

            // For other token types, attempt to get string value
            return reader.GetString() ?? string.Empty;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            // Write as string to keep consumer behavior consistent
            writer.WriteStringValue(value);
        }
    }
}
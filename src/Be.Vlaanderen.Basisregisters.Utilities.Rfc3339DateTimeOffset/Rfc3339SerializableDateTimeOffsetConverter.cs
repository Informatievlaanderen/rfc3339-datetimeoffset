namespace Be.Vlaanderen.Basisregisters.Utilities
{
    using System;
    using Newtonsoft.Json;

    public class Rfc3339SerializableDateTimeOffsetConverter : JsonConverter<Rfc3339SerializableDateTimeOffset?>
    {
        public override void WriteJson(
            JsonWriter writer,
            Rfc3339SerializableDateTimeOffset? value,
            JsonSerializer serializer)
        {
            if (!value.HasValue)
                return;

            var dateTimeOffset = (DateTimeOffset)value;

            serializer.Serialize(writer, dateTimeOffset);
        }

        public override Rfc3339SerializableDateTimeOffset? ReadJson(
            JsonReader reader,
            Type objectType,
            Rfc3339SerializableDateTimeOffset? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
                throw new JsonReaderException($"Expected Date, got {reader.Value}");

            if (reader.TokenType == JsonToken.Date || reader.TokenType == JsonToken.String)
            {
                switch (serializer.DateParseHandling)
                {
                    case DateParseHandling.None:
                        var parsedDateTimeOffset = DateTimeOffset.Parse((string) reader.Value);
                        return new Rfc3339SerializableDateTimeOffset(parsedDateTimeOffset);

                    case DateParseHandling.DateTime:
                        var dateTime = new DateTimeOffset((DateTime) reader.Value);
                        return new Rfc3339SerializableDateTimeOffset(dateTime);

                    case DateParseHandling.DateTimeOffset:
                        var dateTimeOffset = (DateTimeOffset) reader.Value;
                        return new Rfc3339SerializableDateTimeOffset(dateTimeOffset);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(serializer.DateParseHandling));
                }
            }

            throw new JsonReaderException($"Unexcepted token {reader.TokenType}");
        }
    }
}

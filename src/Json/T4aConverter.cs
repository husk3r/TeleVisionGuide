using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeleVisionGuide.Json
{
    public static class T4aSerialize
    {
        public static string ToJson(this T4aRoot self) => JsonSerializer.Serialize(self, T4aConverter.Settings);
    }

    public static class T4aConverter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                T4aNumerationSourceConverter.Singleton,
                T4aNumerationTypeConverter.Singleton,
                T4aTextsUnionConverter.Singleton,
                T4aLangConverter.Singleton,
                T4aTypeEnumConverter.Singleton,
                T4aLevelConverter.Singleton,
                T4aOrientationConverter.Singleton,
                new T4aDateOnlyConverter(),
                new T4aTimeOnlyConverter(),
                T4aIsoDateTimeOffsetConverter.Singleton
            },
        };
    }
    public class T4aNumerationSourceConverter : JsonConverter<T4aNumerationSource>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aNumerationSource);

        public override T4aNumerationSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "Media-Press":
                    return T4aNumerationSource.MediaPress;
                case "Sender":
                    return T4aNumerationSource.Sender;
                case "Wunschliste":
                    return T4aNumerationSource.Wunschliste;
            }
            throw new Exception("Cannot unmarshal type NumerationSource");
        }

        public override void Write(Utf8JsonWriter writer, T4aNumerationSource value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case T4aNumerationSource.MediaPress:
                    JsonSerializer.Serialize(writer, "Media-Press", options);
                    return;
                case T4aNumerationSource.Sender:
                    JsonSerializer.Serialize(writer, "Sender", options);
                    return;
                case T4aNumerationSource.Wunschliste:
                    JsonSerializer.Serialize(writer, "Wunschliste", options);
                    return;
            }
            throw new Exception("Cannot marshal type NumerationSource");
        }

        public static readonly T4aNumerationSourceConverter Singleton = new T4aNumerationSourceConverter();
    }

    public class T4aNumerationTypeConverter : JsonConverter<T4aNumerationType>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aNumerationType);

        public override T4aNumerationType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "AbsoluteNumbering":
                    return T4aNumerationType.AbsoluteNumbering;
                case "SeasonNumbering":
                    return T4aNumerationType.SeasonNumbering;
            }
            throw new Exception("Cannot unmarshal type NumerationType");
        }

        public override void Write(Utf8JsonWriter writer, T4aNumerationType value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case T4aNumerationType.AbsoluteNumbering:
                    JsonSerializer.Serialize(writer, "AbsoluteNumbering", options);
                    return;
                case T4aNumerationType.SeasonNumbering:
                    JsonSerializer.Serialize(writer, "SeasonNumbering", options);
                    return;
            }
            throw new Exception("Cannot marshal type NumerationType");
        }

        public static readonly T4aNumerationTypeConverter Singleton = new T4aNumerationTypeConverter();
    }

    public class T4aTextsUnionConverter : JsonConverter<T4aTextsUnion>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aTextsUnion);

        public override T4aTextsUnion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var objectValue = JsonSerializer.Deserialize<T4aTextsClass>(ref reader, options);
                    return new T4aTextsUnion { TextsClass = objectValue };
                case JsonTokenType.StartArray:
                    var arrayValue = JsonSerializer.Deserialize<object[]>(ref reader, options);
                    return new T4aTextsUnion { AnythingArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type TextsUnion");
        }

        public override void Write(Utf8JsonWriter writer, T4aTextsUnion value, JsonSerializerOptions options)
        {
            if (value.AnythingArray != null)
            {
                JsonSerializer.Serialize(writer, value.AnythingArray, options);
                return;
            }
            if (value.TextsClass != null)
            {
                JsonSerializer.Serialize(writer, value.TextsClass, options);
                return;
            }
            throw new Exception("Cannot marshal type TextsUnion");
        }

        public static readonly T4aTextsUnionConverter Singleton = new T4aTextsUnionConverter();
    }

    public class T4aLangConverter : JsonConverter<T4aLang>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aLang);

        public override T4aLang Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == "deu")
            {
                return T4aLang.Deu;
            }
            throw new Exception("Cannot unmarshal type Lang");
        }

        public override void Write(Utf8JsonWriter writer, T4aLang value, JsonSerializerOptions options)
        {
            if (value == T4aLang.Deu)
            {
                JsonSerializer.Serialize(writer, "deu", options);
                return;
            }
            throw new Exception("Cannot marshal type Lang");
        }

        public static readonly T4aLangConverter Singleton = new T4aLangConverter();
    }

    public class T4aTypeEnumConverter : JsonConverter<T4aTypeEnum>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aTypeEnum);

        public override T4aTypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            switch (value)
            {
                case "Long":
                    return T4aTypeEnum.Long;
                case "VeryShort":
                    return T4aTypeEnum.VeryShort;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void Write(Utf8JsonWriter writer, T4aTypeEnum value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case T4aTypeEnum.Long:
                    JsonSerializer.Serialize(writer, "Long", options);
                    return;
                case T4aTypeEnum.VeryShort:
                    JsonSerializer.Serialize(writer, "VeryShort", options);
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly T4aTypeEnumConverter Singleton = new T4aTypeEnumConverter();
    }

    public class T4aParseStringConverter : JsonConverter<long>
    {
        public override bool CanConvert(Type t) => t == typeof(long);

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }

        public static readonly T4aParseStringConverter Singleton = new T4aParseStringConverter();
    }

    public class T4aLevelConverter : JsonConverter<T4aLevel>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aLevel);

        public override T4aLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (value == "Episode")
            {
                return T4aLevel.Episode;
            }
            throw new Exception("Cannot unmarshal type Level");
        }

        public override void Write(Utf8JsonWriter writer, T4aLevel value, JsonSerializerOptions options)
        {
            if (value == T4aLevel.Episode)
            {
                JsonSerializer.Serialize(writer, "Episode", options);
                return;
            }
            throw new Exception("Cannot marshal type Level");
        }

        public static readonly T4aLevelConverter Singleton = new T4aLevelConverter();
    }

    public class T4aOrientationConverter : JsonConverter<T4aOrientation>
    {
        public override bool CanConvert(Type t) => t == typeof(T4aOrientation);

        public override T4aOrientation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            if (value == "Landscape")
            {
                return T4aOrientation.Landscape;
            }
            throw new Exception("Cannot unmarshal type T4aOrientation");
        }

        public override void Write(Utf8JsonWriter writer, T4aOrientation value, JsonSerializerOptions options)
        {
            if (value == T4aOrientation.Landscape)
            {
                JsonSerializer.Serialize(writer, "Landscape", options);
                return;
            }
            throw new Exception("Cannot marshal type T4aOrientation");
        }

        public static readonly T4aOrientationConverter Singleton = new T4aOrientationConverter();
    }

    public class T4aDateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;
        public T4aDateOnlyConverter() : this(null) { }

        public T4aDateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class T4aTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public T4aTimeOnlyConverter() : this(null) { }

        public T4aTimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class T4aIsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string text;


            if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateText = reader.GetString();

            if (string.IsNullOrEmpty(dateText) == false)
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
                }
            }
            else
            {
                return default(DateTimeOffset);
            }
        }

        public static readonly T4aIsoDateTimeOffsetConverter Singleton = new T4aIsoDateTimeOffsetConverter();
    }

}

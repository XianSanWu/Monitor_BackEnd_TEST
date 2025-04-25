using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utilities.Extensions
{
    public class JsonExtension
    {
        /// <summary>
        /// 設定Json預設DateTime格式化
        /// https://blog.csdn.net/kukubashen/article/details/123798040
        /// </summary>
        public class DateTimeJsonConverter(List<string> formats) : JsonConverter<DateTime>
        {
            private readonly List<string> _Formats = formats;
            private readonly string _Format = "yyyy-MM-dd HH:mm:ss";

            // Deserialization
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string? dateString = reader.GetString();
                if (string.IsNullOrWhiteSpace(dateString)) return DateTime.MinValue;

                foreach (var format in _Formats)
                {
                    if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }

                return DateTime.MinValue;
            }


            // Serialization
            public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
            {
                writer.WriteStringValue(date.ToString(_Format, CultureInfo.InvariantCulture));
            }
        }

    }
}

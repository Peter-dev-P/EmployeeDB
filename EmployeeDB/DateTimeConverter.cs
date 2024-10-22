using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace EmployeeDB
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd")); // Formato "YYYY-MM-DD" para la fecha
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()); //regresa la fecha sin mostrar la hora
        }
    }
}

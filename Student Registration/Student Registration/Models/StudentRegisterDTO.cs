﻿using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Student_Registration.Models
{
    public class StudentRegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime Birthdate { get; set; } // Uses YYYY-DD-MM format

        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string GuardianName { get; set; }
        public string GuardianAddress { get; set; }
        public string GuardianContact { get; set; }
        public string Hobby { get; set; }
        public string? Status { get; set; }


        [FromForm]
        public List<IFormFile>? Documents { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
    }

    // Custom Date Converter for YYYY-DD-MM format
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private readonly string DateFormat = "yyyy-dd-MM"; // Expected format sample format 2000-01-12

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            throw new JsonException($"Invalid date format: {reader.GetString()}. Expected format: {DateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat)); // Serializing as YYYY-DD-MM
        }
    }
}

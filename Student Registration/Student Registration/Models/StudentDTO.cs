using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Student_Registration.Models;

namespace Student_Registration.DTOs
{
    public class StudentDTO
    {
        [BindNever]
        public string? StudentCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime Birthdate { get; set; } // Uses YYYY-MM-DD format

        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string GuardianName { get; set; }
        public string GuardianAddress { get; set; }
        public string GuardianContact { get; set; }
        public string Hobby { get; set; }
        public string? Status { get; set; }
        public string? AccountStatus { get; set; }

        public List<StudentDocumentDTO>? Documents { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }

        //public bool isdeleted { get; set; } = false;
       // public string? whendeleted { get; set; }
    }

    // DTO for documents
    public class StudentDocumentDTO
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Data { get; set; } // Convert byte[] to Base64

        public static StudentDocumentDTO FromEntity(StudentDocuments doc)
        {
            return new StudentDocumentDTO
            {
                FileName = doc.FileName,
                FileType = doc.FileType,
                Data = Convert.ToBase64String(doc.Data) // Convert to Base64 for response
            };
        }
    }

    // Custom Date Converter for YYYY-MM-DD format
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private readonly string DateFormat = "yyyy-MM-dd"; // Corrected format

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
            writer.WriteStringValue(value.ToString(DateFormat)); // Serializing as YYYY-MM-DD
        }
    }

}

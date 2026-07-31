using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CreatReports.JSON
{
    internal class EmployeeInfo
    {
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; } = string.Empty;
        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CreatReports.JSON
{
    internal class JSONFileInfo //Класс для сохранения полей JSON с соответствующими контрактами по именам полей
    {
        [JsonPropertyName("documentTitle")]
        public string DocumentTitle { get; set; } = string.Empty;
        [JsonPropertyName("csvFilePath")]
        public string CsvFilePath { get; set; } = string.Empty;
        [JsonPropertyName("employee")]
        public EmployeeInfo Employee { get; set; } = new();

        public bool CheckValues()
        {
            bool result = !string.IsNullOrWhiteSpace(DocumentTitle) && !string.IsNullOrWhiteSpace(CsvFilePath)
                    && Employee != null 
                    && !string.IsNullOrWhiteSpace(Employee.LastName)
                    && !string.IsNullOrWhiteSpace(Employee.FirstName) 
                    && !string.IsNullOrWhiteSpace(Employee.MiddleName) 
                    && !string.IsNullOrWhiteSpace(Employee.Position);

            return result;
        }
    }
}

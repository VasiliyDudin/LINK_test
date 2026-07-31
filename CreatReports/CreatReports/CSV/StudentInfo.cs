using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreatReports.CSV
{
    internal class StudentInfo
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public int Evaluation { get; set; }
    }

    internal sealed class StudentInfoMapName : ClassMap<StudentInfo> //делаем мапинг по названиям из первой строки CSV файла
    {
        public StudentInfoMapName()
        {
            Map(m => m.LastName).Name("Фамилия");
            Map(m => m.FirstName).Name("Имя");
            Map(m => m.Evaluation).Name("Оценка");
        }
    }
}

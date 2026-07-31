using CreatReports.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreatReports
{
    internal class Errors
    {
        public enum Code
        {
            FileNotFound = 1,
            InvalidInput = 2,
            ConnectionFailed = 3,
            JSONError = 4,
            CSVError = 5,
            GenerateError = 6
        }

        public static string GetMessage(Code code)
        {
            string result = string.Empty;

            switch (code)
            {
                case Code.FileNotFound:
                    result = "Ошибка !\r\nФайл не найден.";
                    break;
                case Code.InvalidInput:
                    result = "Ошибка !\r\nЗначение введено не корректно.";
                    break;
                case Code.ConnectionFailed:
                    result = "Ошибка подключения к БД.";
                    break;
                case Code.JSONError:
                    result = "Ошибка в заполнении полей JSON файла.";
                    break;
                case Code.CSVError:
                    result = "Ошибка !\r\nПроверьте корректность полей CSV файла.";
                    break;
                case Code.GenerateError:
                    result = "Ошибка !\r\nГенерации word файла.";
                    break;
                default:
                    result = "Неизвестный код ошибки.";
                    break;
            };

            return result;
        }
    }
}

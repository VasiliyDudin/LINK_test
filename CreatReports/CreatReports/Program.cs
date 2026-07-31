using CreatReports;
using CreatReports.CSV;
using CreatReports.JSON;
using CsvHelper;
using System.Globalization;
using System.Text.Json;

internal class Program
{
    const string _info = "Данная программа предназначена для создания отчетности.";
    const string _startInfo = "Для начала использования введите полный путь к конфигурационному файлу в формате JSON :";
    const string _nameTemplate = "\\SourceTemplate.docx";

    private static async Task Main(string[] args)
    {
        bool isRun = true;
        string? input = string.Empty, path = string.Empty;
        List<StudentInfo> students = new List<StudentInfo>();

        Console.WriteLine(_info);

        while (isRun)
        {
            Console.WriteLine(_startInfo);
            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.InvalidInput));
                continue;
            }

            if (!File.Exists(input))
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.FileNotFound) + $"\r\n{input}");
                isRun = false;
                continue;
            }

            JSONFileInfo info = await GetFileInfoFromJSONAsync(input); //Получаем данные из JSON файла конфигурации
            if (!info.CheckValues())
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.JSONError) + $"\r\n{input}");
                isRun = false;
                continue;
            }

            Console.WriteLine($"\r\nDeserialize файла - {input} прошла успешно !");

            path = Path.GetDirectoryName(input) + info.CsvFilePath.Replace('/', '\\'); //Предполагаем, что Csv файла расположен относительно директории файла конфигурации
            students = await GetStudentsFromCsvAsync(path); //Получаем данные из Csv файла

            if (students == null || students.Count == 0)
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.CSVError) + $"\r\n{info.CsvFilePath}");
                isRun = false;
                continue;

            }

            Console.WriteLine($"\r\nЗагрузка данных из файла - {info.CsvFilePath} прошла успешно !");
            Console.WriteLine("Введите полный путь с названием, куда нужно сохранить сгенерированный файл :");

            path = Console.ReadLine();

            CreatorWord creator = new CreatorWord(Path.GetDirectoryName(input) + _nameTemplate, path, info, students); //Предполагаем, что шаблон лежит по тому же пути где расположен файл конфигурации с неизменяемым названием - SourceTemplate.docx

            if (!await creator.GenerateDocumentAsync())
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.GenerateError));
                continue;
            }

            Console.WriteLine($"\r\nНовый файла - {path} сгенерирован успешно !");

            Console.Write("Для генерации еще одного документа нажмите - (Y) или любую клавишу для завершения работы : ");
            input = Console.ReadLine();

            isRun = input?.ToUpper() == "Y" ? true: false;
        }
    }

    static async Task<JSONFileInfo> GetFileInfoFromJSONAsync(string path)
    {
        await using var fileStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<JSONFileInfo>(fileStream) ?? new JSONFileInfo();
    }

    static async Task<List<StudentInfo>> GetStudentsFromCsvAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"{Errors.GetMessage(Errors.Code.FileNotFound)}\n{filePath}");
            return new List<StudentInfo>();
        }

        try
        {
            using StreamReader reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<StudentInfoMapName>();

            List<StudentInfo> students = new List<StudentInfo>();
            await foreach (var s in csv.GetRecordsAsync<StudentInfo>())
            {
                students.Add(s);
            }

            return students;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение - {ex.Message}");
            return new List<StudentInfo>();
        }
    }
}
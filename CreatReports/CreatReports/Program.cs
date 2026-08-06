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
    const string _folderTemplate = "Templates";
    const string _nameTemplate = "SourceTemplate.docx";

    private static async Task Main(string[] args)
    {
        bool isRun = true;
        string? pathCSV = string.Empty, pathConf = string.Empty;
        List<StudentInfo> students = new List<StudentInfo>();

        bool isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"; //Для определения окружения

        Console.WriteLine(_info);

        while (isRun)
        {
            Console.WriteLine(_startInfo);
            pathConf = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(pathConf))
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.InvalidInput));
                continue;
            }

            if (!File.Exists(pathConf))
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.FileNotFound) + $"\r\n{pathConf}");
                isRun = false;
                continue;
            }

            JSONFileInfo info = await GetFileInfoFromJSONAsync(pathConf);
            if (!info.CheckValues())
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.JSONError) + $"\r\n{pathConf}");
                isRun = false;
                continue;
            }

            Console.WriteLine($"\r\nDeserialize файла - {pathConf} прошла успешно !");


            string? baseDir = Path.GetDirectoryName(pathConf);
            if (isInContainer)
            {
                pathCSV = baseDir + "/" + info.CsvFilePath.TrimStart('/');
            }
            else
            {
                pathCSV = Path.GetDirectoryName(pathConf) + info.CsvFilePath.Replace('/', '\\');
            }

            students = await GetStudentsFromCsvAsync(pathCSV);

            if (students == null || students.Count == 0)
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.CSVError) + $"\r\n{info.CsvFilePath}");
                isRun = false;
                continue;
            }

            Console.WriteLine($"\r\nЗагрузка данных из файла - {info.CsvFilePath} прошла успешно !");
            Console.WriteLine("Введите полный путь с названием, куда нужно сохранить сгенерированный файл :");

            pathCSV = Console.ReadLine();


            string templatePath;
            if (isInContainer)
            {
                // В Docker — шаблон лежит в /app/data/Templates/
                templatePath = "/app/data/" + _folderTemplate + "/" + _nameTemplate;
            }
            else
            {
                templatePath = Path.GetDirectoryName(pathConf) + "\\" + _folderTemplate + "\\" + _nameTemplate;
            }

            CreatorWord creator = new CreatorWord(templatePath, pathCSV, info, students);

            if (!await creator.GenerateDocumentAsync())
            {
                Console.WriteLine(Errors.GetMessage(Errors.Code.GenerateError));
                continue;
            }

            Console.WriteLine($"\r\nНовый файл - {pathCSV} сгенерирован успешно !");

            Console.Write("Для генерации еще одного документа нажмите - (Y) или любую клавишу для завершения работы : ");

            isRun = Console.ReadLine().ToUpper() == "Y" ? true: false;

            if (isRun)
            {
                if (!await creator.SetIncrDocumentNumberAsync(pathConf))
                {
                    Console.WriteLine(Errors.GetMessage(Errors.Code.ConfigError) + "\r\n Работа с программой не может быть продолжена !");
                    return;
                }
            }
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
using CreatReports.CSV;
using CreatReports.Interfaces;
using CreatReports.JSON;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace CreatReports
{
    internal class CreatorWord : ICreator
    {
        string? _pathTemplate, _pathOutput;
        JSONFileInfo _info;
        List<StudentInfo> _students;

        public CreatorWord(string pathTemplate, string pathOutput, JSONFileInfo info, List<StudentInfo> students)
        {
            _pathTemplate = pathTemplate;
            _pathOutput = pathOutput;
            _info = info;
            _students = students;
        }

        public async Task<bool> GenerateDocumentAsync()
        { 
            bool result = true;

            if (string.IsNullOrWhiteSpace(_pathTemplate))
                return false;

            if (string.IsNullOrWhiteSpace(_pathOutput)) //Если путь, куда должен быть сохранен новый файл, не указан, сохраняем туда же, где лежит шаблон
            {
                _pathOutput = Path.GetDirectoryName(_pathTemplate);
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(_pathOutput)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_pathOutput));
            }

            try
            {
                using DocX doc = DocX.Load(_pathTemplate);

                ReplacePlaceholders(doc);

                FillTable(doc);

                doc.SaveAs(_pathOutput);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение - {ex.Message}");
                result = false;
            }

            return result;
        }

        public void ReplacePlaceholders(DocX document)
        {
            document.ReplaceText("{DocumentTitle}", _info.DocumentTitle);
            document.ReplaceText("{LastName}", _info.Employee.LastName);
            document.ReplaceText("{FirstName}", _info.Employee.FirstName.ToUpper().Substring(0,1)); //т.к. требуется первая буква имени
            document.ReplaceText("{MiddleName}", _info.Employee.MiddleName.ToUpper().Substring(0, 1));
            document.ReplaceText("{Position}", _info.Employee.Position);
        }

        public async Task<bool> SetIncrDocumentNumberAsync(string pathConf)
        {
            string docNumb = string.Empty;
            string title = _info.DocumentTitle;

            if(!title.Contains('№'))
                return false;

            string numbTitle = title.Substring(title.LastIndexOf(' ')).Trim();
            string subTitle = title.Substring(0, title.LastIndexOf('№') + 1);

            var match = Regex.Match(numbTitle, @"\d+");

            docNumb = match.Value;

            if (string.IsNullOrWhiteSpace(docNumb) || !int.TryParse(docNumb, out int number))
                return false;

            _info.DocumentTitle = subTitle + ++number;

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(_info, options);
                await File.WriteAllTextAsync(pathConf, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение - {ex.Message}");
                return false;
            }

            return true;
        }

        public void FillTable(DocX document)
        {
            Table table = document.Tables[0];
            Row lastRow = table.Rows.Last();
            const int sourceRows = 3; //Количество строк исходной таблицы
            int removeIndx = table.Rows.Count - 1; //Запоменаем индекс строки с Placeholder для последующего удаления

            foreach (var student in _students)
            {
                var newRow = table.InsertRow(lastRow);

                newRow.Cells[0].Paragraphs.First().ReplaceText("{RowNumberTbl}", (table.Rows.Count - sourceRows).ToString());
                newRow.Cells[1].Paragraphs.First().ReplaceText("{LastNameTbl}", student.LastName);
                newRow.Cells[2].Paragraphs.First().ReplaceText("{FirstNameTbl}", student.FirstName);
                newRow.Cells[3].Paragraphs.First().ReplaceText("{EvaluationTbl}", student.Evaluation.ToString());
            }

            table.RemoveRow(removeIndx);
        }
    }
}

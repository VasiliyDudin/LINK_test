using System;
using System.Collections.Generic;
using System.Text;
using Xceed.Words.NET;
using static CreatReports.Errors;

namespace CreatReports.Interfaces
{
    internal interface ICreator
    {
        Task<bool> GenerateDocumentAsync();
        void ReplacePlaceholders(DocX document);
        void FillTable(DocX document);
    }
}

namespace PDFSwissKnife.Services;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Collections.Generic;
using System.IO;

public class PdfLogic
{
    public void MergePdfs(IEnumerable<string> filePaths, string outputPath)
    {
        using (var outputDocument = new PdfDocument())
        {
            foreach (var filePath in filePaths)
            {
                var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                foreach (var page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
            }
            outputDocument.Save(outputPath);
        }
    }

    public void SplitPdf(string filePath, string outputDirectory)
    {
        var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
        for (int i = 0; i < inputDocument.PageCount; i++)
        {
            var outputDocument = new PdfDocument();
            outputDocument.AddPage(inputDocument.Pages[i]);
            var outputPath = Path.Combine(outputDirectory, $"Page_{i + 1}.pdf");
            outputDocument.Save(outputPath);
        }
    }
}


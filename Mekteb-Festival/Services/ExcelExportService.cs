using ClosedXML.Excel;
using Mekteb_Festival.Data;
using System.IO;

namespace Mekteb_Festival.Services
{
    public class ExcelExportService
    {
        public byte[] GenerateExcel(List<Registration> prijave)
        {
            using var workbook = new XLWorkbook();
            
            int odrasli = prijave.Sum(r => r.BrojOdraslih);
            int malaDjeca = prijave.Sum(r => r.MaliDjeca);
            int takmicari = prijave.Sum(r => r.Takmicari.Count);
            int ukupno = odrasli + malaDjeca + takmicari;
            var worksheet = workbook.Worksheets.Add("Prijave");
            worksheet.Cell(1, 1).Value = $"Statistika za {DateTime.Now:dd.MM.yyyy}";
            worksheet.Cell(2, 1).Value = $"Ukupno učesnika: {ukupno}";
            worksheet.Cell(3, 1).Value = $"• Odrasli: {odrasli}";
            worksheet.Cell(4, 1).Value = $"• Djece ukupno: {takmicari+malaDjeca}";
            worksheet.Cell(5, 1).Value = $" -- Mekteblije: {takmicari}";
            worksheet.Cell(6, 1).Value = $" -- Mala djeca: {malaDjeca}";
            // Header
            worksheet.Cell(8, 1).Value = "Džemat";
            worksheet.Cell(8, 2).Value = "Ime i prezime";
            worksheet.Cell(8, 3).Value = "Broj odraslih";
            worksheet.Cell(8, 4).Value = "Mekteblije";
            worksheet.Cell(8, 5).Value = "Broj ostale djece";

            int row = 9;

            foreach (var r in prijave)
            {
                worksheet.Cell(row, 1).Value = r.Dzemat;
                worksheet.Cell(row, 2).Value = r.ImePrezime;
                worksheet.Cell(row, 3).Value = r.BrojOdraslih;
                worksheet.Cell(row, 4).Value = string.Join(", ", r.Takmicari.Select(t => t.ImePrezime));
                worksheet.Cell(row, 5).Value = r.MaliDjeca;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}

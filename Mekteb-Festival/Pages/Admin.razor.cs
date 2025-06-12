using Mekteb_Festival.Data;
using Mekteb_Festival.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Mekteb_Festival.Pages
{
    [Authorize]
    public partial class Admin : ComponentBase
    {
        private List<Registration> svePrijave = new();
        private string selectedDzemat = "";
        [Inject] public DzematService DzematService { get; set; } = default!;
        [Inject] public ApplicationDbContext Db { get; set; } = default!;
        [Inject] public ExcelExportService ExcelExporter { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;
        [Inject] public PdfExportService PdfExporter { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        private List<string> Dzemati = new();
        private int ukupnoOdraslih => FiltriranePrijave.Sum(r => r.BrojOdraslih);
        private int ukupnoMalaDjeca => FiltriranePrijave.Sum(r => r.MaliDjeca);
        private int ukupnoTakmicara => FiltriranePrijave.Sum(r => r.Takmicari.Count);
        private int ukupnoUcesnika => ukupnoOdraslih + ukupnoMalaDjeca + ukupnoTakmicara;
        private IEnumerable<Registration> FiltriranePrijave =>
            string.IsNullOrWhiteSpace(selectedDzemat)
                ? svePrijave
                : svePrijave.Where(p => p.Dzemat == selectedDzemat);

        protected override async Task OnInitializedAsync()
        {
            Dzemati = DzematService.GetDzemati();
            svePrijave = await Db.Registrations.Include(r => r.Takmicari).OrderBy(x=> x.Dzemat).ThenBy(x=> x.ImePrezime).ToListAsync();
        }

        private async Task ExportExcel()
        {
            var data = FiltriranePrijave.ToList();
            var bytes = ExcelExporter.GenerateExcel(data);
            var naziv = string.IsNullOrWhiteSpace(selectedDzemat) ? "Svi" : selectedDzemat;

            var fileName = $"Prijave_{naziv}_{DateTime.Now:yyyyMMddHHmm}.xlsx";
            var base64 = Convert.ToBase64String(bytes);
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            await JS.InvokeVoidAsync("downloadFile", fileName, base64, mimeType);
        }

        private async Task ExportPdf()
        {
            var data = FiltriranePrijave.ToList();
            var bytes = PdfExporter.GeneratePdf(data);
            var naziv = string.IsNullOrWhiteSpace(selectedDzemat) ? "Svi" : selectedDzemat;

            var fileName = $"Prijave_{naziv}_{DateTime.Now:yyyyMMddHHmm}.pdf";
            var base64 = Convert.ToBase64String(bytes);
            var mimeType = "application/pdf";

            await JS.InvokeVoidAsync("downloadFile", fileName, base64, mimeType);
        }
    }
}
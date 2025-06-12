using Mekteb_Festival.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mekteb_Festival.Services
{
    public class PdfExportService
    {
        public byte[] GeneratePdf(List<Registration> prijave)
        {
            int odrasli = prijave.Sum(r => r.BrojOdraslih);
            int malaDjeca = prijave.Sum(r => r.MaliDjeca);
            int takmicari = prijave.Sum(r => r.Takmicari.Count);
            int ukupno = odrasli + malaDjeca + takmicari;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Prijave za Mekteb Festival")
                        .FontSize(16)
                        .Bold()
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Statistika ({DateTime.Now:dd.MM.yyyy})").Bold().FontSize(14);
                        col.Item().Text($"Ukupno učesnika: {ukupno}");
                        col.Item().Text($"• Odrasli: {odrasli}");
                        col.Item().Text($"• Djece ukupno: {takmicari+malaDjeca}");
                        col.Item().Text($" - Mekteblije: {takmicari}");
                        col.Item().Text($" - Mala djeca: {malaDjeca}");

                        col.Item().PaddingTop(10);
                        foreach (var p in prijave)
                        {
                            col.Item().PaddingBottom(5).Element(c => RenderEntry(c, p));
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Stranica ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        private void RenderEntry(IContainer container, Registration p)
        {
            container.BorderBottom(1).PaddingBottom(5).Column(col =>
            {
                col.Item().Text($"Džemat: {p.Dzemat}").Bold();
                col.Item().Text($"Ime i prezime: {p.ImePrezime}");
                col.Item().Text($"Odrasli: {p.BrojOdraslih}");
                col.Item().Text($"Broj ostale djece: {p.MaliDjeca}");

                if (p.Takmicari.Any())
                {
                    col.Item().Text("Mekteblije:").SemiBold();

                    col.Item().Column(inner =>
                    {
                        foreach (var t in p.Takmicari)
                        {
                            inner.Item().Text($"- {t.ImePrezime}");
                        }
                    });
                }
            });
        }
    }
}

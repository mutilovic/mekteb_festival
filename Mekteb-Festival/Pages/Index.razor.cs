using Mekteb_Festival.Data;
using Mekteb_Festival.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.IO;

namespace Mekteb_Festival.Pages
{
    public partial class Index : ComponentBase
    {
        private Registration model = new();
        private bool success = false;
        [Inject] public DzematService DzematService { get; set; } = default!;
        [Inject] public ApplicationDbContext Db { get; set; } = default!;
        [Inject] public ILogger<Index> Logger { get; set; } = default!;
        private List<string> Dzemati = new();
        private string? errorMessage;
        private int ukupnoPrijavljenih = 0;
        private Dictionary<string, int> dzematStats = new();
        private static int dbHits = 0;

        private readonly string registracijeCachePath = "wwwroot/temp/registracijeCache.json";
        private readonly string dzematStatsCachePath = "wwwroot/temp/dzematStatsCache.json";

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("OnInitializedAsync gestartet");
            Dzemati = DzematService.GetDzemati();

            model = new Registration
            {
                Takmicari = new List<Takmicar> { new(), new(), new() }
            };
            string directoryPath = Path.GetDirectoryName(registracijeCachePath)!;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // Verzeichnis erstellen, falls es nicht existiert
            }
            // Versuche Cache aus Datei zu lesen
            List<Registration> registracije;
            if (File.Exists(registracijeCachePath))
            {
                Logger.LogInformation("RegistracijeCache aus Datei geladen.");
                var json = await File.ReadAllTextAsync(registracijeCachePath);
                registracije = JsonSerializer.Deserialize<List<Registration>>(json) ?? new List<Registration>();
            }
            else
            {
                // Hole aus DB und speichere in Datei
                registracije = await Db.Registrations.Include(r => r.Takmicari).ToListAsync();
                var json = JsonSerializer.Serialize(registracije);
                await File.WriteAllTextAsync(registracijeCachePath, json);
                Logger.LogWarning("EF-Zugriff Nr. {Count}", Interlocked.Increment(ref dbHits));
            }

            ukupnoPrijavljenih = registracije.Sum(r => r.BrojOdraslih + r.MaliDjeca + r.Takmicari.Count);

            // Versuche DzematStats aus Datei zu lesen
            Dictionary<string, int> stats;
            if (File.Exists(dzematStatsCachePath))
            {
                Logger.LogInformation("DzematStatsCache aus Datei geladen.");
                var json = await File.ReadAllTextAsync(dzematStatsCachePath);
                stats = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
            }
            else
            {
                // Berechne die DzematStats und speichere sie in Datei
                stats = Dzemati
                    .Select(dz => new
                    {
                        Dzemat = dz,
                        Broj = registracije
                                .Where(r => r.Dzemat == dz)
                                .Sum(r => r.BrojOdraslih + r.MaliDjeca + r.Takmicari.Count)
                    })
                    .Where(x => x.Broj > 0).OrderByDescending(x => x.Broj)
                    .ToDictionary(x => x.Dzemat, x => x.Broj);

                var json = JsonSerializer.Serialize(stats);
                await File.WriteAllTextAsync(dzematStatsCachePath, json);
                Logger.LogWarning("EF-Zugriff Nr. {Count}", Interlocked.Increment(ref dbHits));
            }

            dzematStats = stats;
        }

        private void AddTakmicar()
        {
            model.Takmicari.Add(new Takmicar());
        }

        private void RemoveTakmicar(Takmicar t)
        {
            model.Takmicari.Remove(t);
        }

        private async Task HandleValidSubmit()
        {
            model.Takmicari = model.Takmicari
                .Where(t => !string.IsNullOrWhiteSpace(t.ImePrezime))
                .ToList();

            if (model.Takmicari.Any())
            {
                Db.Registrations.Add(model);
                await Db.SaveChangesAsync();

                Logger.LogInformation("HandleValidSubmit ausgeführt");
                // Cache invalidieren (neuladen beim nächsten Zugriff)
                if (File.Exists(registracijeCachePath)) File.Delete(registracijeCachePath);
                if (File.Exists(dzematStatsCachePath)) File.Delete(dzematStatsCachePath);

                model = new Registration
                {
                    Takmicari = new List<Takmicar> { new(), new(), new() }
                };
                success = true;
                errorMessage = null;
            }
            else
            {
                success = false;
                errorMessage = "Unesite barem jedno dijete mektebskog uzrasta.";
            }
        }
    }
}

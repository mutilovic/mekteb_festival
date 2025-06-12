namespace Mekteb_Festival.Services;

public class DzematService
{
    private readonly List<string> _dzemati;

    public DzematService(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.WebRootPath, "data", "dzemati.txt");

        if (File.Exists(path))
        {
            _dzemati = File.ReadAllLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToList();
        }
        else
        {
            _dzemati = new List<string>();
        }
    }

    public List<string> GetDzemati() => _dzemati;
}
using Mekteb_Festival.Data;
using System.ComponentModel.DataAnnotations;

public class Registration
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Džemat je obavezan.")]
    public string Dzemat { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ime i prezime staratelja je obavezno.")]
    public string ImePrezime { get; set; } = string.Empty;


    [Range(0, 100)]
    public int BrojOdraslih { get; set; }

    public List<Takmicar> Takmicari { get; set; } = new();

    [Range(0, 100)]
    public int MaliDjeca { get; set; }
}

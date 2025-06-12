using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mekteb_Festival.Data
{
    public class Takmicar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime i prezime djeteta mektebskog uzrasta je obavezno.")]
        public string ImePrezime { get; set; } = string.Empty;

        public int RegistrationId { get; set; }

        [JsonIgnore]
        public Registration? Registration { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;
using Katastar.Data;

namespace Katastar.Models
{
    public class Predmet
    {
        [Key]
        public int Id { get; set; }

        public string? Ime { get; set; }

        public string? Vlasnik { get; set; }

        public string? Lokacija { get; set; }

        public string? Dimenzija { get; set; }

        public string? Deskripcija { get; set; }

        public bool Zavrsen { get; set; }

        public string? SluzbenikId { get; set; }
        public ApplicationUser? Sluzbenik { get; set; }
    }
}

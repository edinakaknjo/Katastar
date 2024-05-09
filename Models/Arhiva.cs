using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Katastar.Data;
using Katastar.Models;

namespace Katastar.Models
{
    public class Arhiva
    {
        [Key]
        public int Id { get; set; }

        public string SluzbenikId { get; set; } 
        public ApplicationUser Sluzbenik { get; set; } 

        public DateTime DatumDodavanja { get; set; } 

        public string Ime { get; set; }
        public string Vlasnik { get; set; }
        public string Lokacija { get; set; }
        public string Dimenzija { get; set; }
        public string Deskripcija { get; set; }
    }

}

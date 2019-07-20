using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    [Table("IPDetails")]
    public class IPDetailsDTO
    {
        [StringLength(50)]
        [Required]
        [Key]
        public string IP { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string Continent { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public double Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public double Longitude { get; set; }
    }
}

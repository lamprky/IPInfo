using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Database
{
    [Table("BatchDetails")]
    public class BatchDetailsDTO
    {
        [Key]
        public Guid ID { get; set; }
        public int No_of_Updates { get; set; }
        public int No_of_Updates_Processed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace HattingtonGame.DbTypes
{
    public class Hat
    {
        [Key]
        public int HatID { get; set; }

        [Required]
        [StringLength(30)]
        public string HatName { get; set; }

        [Required]
        public int HatTierID { get; set; }

    }
}

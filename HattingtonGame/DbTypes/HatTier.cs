using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class HatTier
    {
        [Key]
        public int HatTierID { get; set; }

        [Required]
        public int MinLevel { get; set; }

        [Required]
        public int MaxLevel { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HattingtonGame.DbTypes
{
    public class Food
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ItemTypeID { get; set; }

        [Required]
        public int Energy { get; set; }

        [Required]
        public int AppearanceChance { get; set; }

    }
}

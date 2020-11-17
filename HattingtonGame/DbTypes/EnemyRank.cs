using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class EnemyRank
    {
        [Key]
        public int EnemyRankID { get; set; }

        [Required]
        public int MinLevel { get; set; }

        [Required]
        public int MaxLevel { get; set; }
    }
}

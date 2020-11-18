using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class Enemy
    {
        [Key]
        public int EnemyID { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        public int EnemyRankID { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public int MaxHealth { get; set; }

        [Required]
        public int Attack { get; set; }

        [Required]
        public int Defense { get; set; }

        [Required]
        public int Magic { get; set; }

        [Required]
        public int MagicDefense { get; set; }

        [Required]
        public int NormalAttackChances { get; set; }

        [Required]
        public int MagicAttackChances { get; set; }

        [Required]
        public int ExpGain { get; set; }
    }
}

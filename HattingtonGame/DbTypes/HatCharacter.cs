using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class HatCharacter
    {
        [Key]
        public int HatCharacterID { get; set; }

        [Required]
        [StringLength(20)]
        public string CharacterName { get; set; }

        [Required]
        [StringLength(50)]
        public string DiscordUser { get; set; }

        [Required]
        public int HatID { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public int Experience { get; set; }

        [Required]
        public int Health { get; set; }

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
        public int Stamina { get; set; }

        [Required]
        public int MaxStamina { get; set; }

        [Required]
        public int Fullness { get; set; }

        [Required]
        public int MaxFullness { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string DiscordUser { get; set; }

        [Required]
        [StringLength(200)]
        public string FoodItemsString { get; set; }

    }
}

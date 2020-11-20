using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HattingtonGame.DbTypes
{
    public class ItemType
    {
        [Key]
        public int ItemTypeID { get; set; }

        [Required]
        [StringLength(20)]
        public string TypeName { get; set; }
    }
}

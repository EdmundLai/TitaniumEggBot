using System;
using System.Collections.Generic;
using System.Text;

namespace TitaniumEggBot.Rotmg
{
    public class RotmgCharacter
    {
        public string Class { get; set; }
        public int Level { get; set; }
        public int RankedPlace { get; set; }
        public int Fame { get; set; }
        public string Stats { get; set; }
        public RotmgEquipment Equipment { get; set; }
    }
}

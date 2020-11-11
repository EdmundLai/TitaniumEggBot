using System;
using System.Collections.Generic;
using System.Text;

namespace TitaniumEggBot.Rotmg
{
    public class RotmgPlayer
    {
        public string PlayerName { get; set; }
        public string RealmeyeURL { get; set; }
        public int Rank { get; set; }
        public List<RotmgCharacter> Characters { get; set; }
    }
}

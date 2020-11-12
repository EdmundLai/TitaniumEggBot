using System;
using System.Collections.Generic;
using System.Text;

namespace TitaniumEggBot.LeagueOfLegends
{
    public class MatchStats
    {
        public bool Win { get; set; }
        public string Champion { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
    }
}

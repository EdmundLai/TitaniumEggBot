using System;
using System.Collections.Generic;
using System.Text;

namespace TitaniumEggBot.LeagueOfLegends
{
    public class MatchHistory
    {
        public string SummonerName { get; set; }

        // set to null if match list is null
        public List<MatchStats> Matches { get; set; }
    }
}

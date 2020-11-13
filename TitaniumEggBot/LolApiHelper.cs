using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitaniumEggBot.LeagueOfLegends;

namespace TitaniumEggBot
{
    public class LolApiHelper
    {
        private RiotApi _riotApi;

        public LolApiHelper(AppSecrets appSecrets)
        {
            _riotApi = RiotApi.NewInstance(appSecrets.LolApiToken);
        }

        public async Task<string> GetPlayerRankNAAsync(string summonerName)
        {
            // Get summoner.
            var summonerData = await _riotApi.SummonerV4.GetBySummonerNameAsync(Region.NA, summonerName);
            // Get league positions (all ranks).
            var ranks = await _riotApi.LeagueV4.GetLeagueEntriesForSummonerAsync(Region.NA, summonerData.Id);
            // Find solo queue rank.
            var soloRank = ranks.FirstOrDefault(r => r.QueueType == "RANKED_SOLO_5x5");
            if (soloRank == null)
                return $"{ summonerData.Name} is unranked.";
            else
                return $"{summonerData.Name} is {soloRank.Tier} {soloRank.Rank}";
        }

        public async Task<MatchHistory> GetMatchHistoryAsync(string summonerName, int queueType)
        {
            var summonerData = await _riotApi.SummonerV4.GetBySummonerNameAsync(Region.NA, summonerName);

            if(summonerData is null)
            {
                // return null data if summoner cannot be found
                return null;
            } else
            {
                // 450 is ARAM
                var matchlist = await _riotApi.MatchV4.GetMatchlistAsync(Region.NA, summonerData.AccountId, queue: new[] { queueType }, endIndex: 5);

                // in game name
                string ign = summonerData.Name;

                if(matchlist is null)
                {
                    // return history with null match list
                    return new MatchHistory { SummonerName = ign };
                } else
                {
                    // Get match results (done asynchronously -> not blocking -> fast).
                    var matchDataTasks = matchlist.Matches.Select(
                           matchMetadata => _riotApi.MatchV4.GetMatchAsync(Region.NA, matchMetadata.GameId)
                       ).ToArray();
                    // Wait for all task requests to complete asynchronously.
                    var matchDatas = await Task.WhenAll(matchDataTasks);

                    //StringBuilder sb = new StringBuilder($"Match history for {summonerData.Name}:\n");

                    List<MatchStats> matchStatsList = new List<MatchStats>();

                    for (var i = 0; i < matchDatas.Count(); i++)
                    {
                        var matchData = matchDatas[i];
                        // Get this summoner's participant ID info.
                        var participantIdData = matchData.ParticipantIdentities
                            .First(pi => summonerData.Id.Equals(pi.Player.SummonerId));
                        // Find the corresponding participant.
                        var participant = matchData.Participants
                            .First(p => p.ParticipantId == participantIdData.ParticipantId);

                        var win = participant.Stats.Win;
                        var champ = (Champion)participant.ChampionId;
                        var k = participant.Stats.Kills;
                        var d = participant.Stats.Deaths;
                        var a = participant.Stats.Assists;
                        var kda = (k + a) / (float)d;

                        MatchStats stats = new MatchStats
                        {
                            Champion = champ.Name(),
                            Win = win,
                            Kills = k,
                            Deaths = d,
                            Assists = a
                        };

                        matchStatsList.Add(stats);
                    }

                    return new MatchHistory
                    {
                        SummonerName = ign,
                        Matches = matchStatsList
                    };
                }
            }
        }

        public static string CalculateKDA(int kills, int deaths, int assists)
        {
            var kda = (kills + assists) / (float)deaths;
            return kda.ToString("F2");
        }
    }
}

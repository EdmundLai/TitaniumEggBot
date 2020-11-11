﻿using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public async Task<string> GetMatchHistoryStringAsync(string summonerName, int queueType)
        {
			var summonerData = await _riotApi.SummonerV4.GetBySummonerNameAsync(Region.NA, summonerName);

			// 450 is ARAM
			var matchlist = await _riotApi.MatchV4.GetMatchlistAsync(Region.NA, summonerData.AccountId, queue: new[] { queueType }, endIndex: 10);


			// Get match results (done asynchronously -> not blocking -> fast).
			var matchDataTasks = matchlist.Matches.Select(
				   matchMetadata => _riotApi.MatchV4.GetMatchAsync(Region.NA, matchMetadata.GameId)
			   ).ToArray();
			// Wait for all task requests to complete asynchronously.
			var matchDatas = await Task.WhenAll(matchDataTasks);

			StringBuilder sb = new StringBuilder($"Match history for {summonerData.Name}:\n");

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

				string winOrLoss = win ? "Win" : "Loss";

				// Print #, win/loss, champion.
				string firstLine = $"{i + 1}) {winOrLoss} ({champ.Name()})\n";
				// Print champion, K/D/A
				string secondLine = $"     K/D/A {k}/{d}/{a} {kda.ToString("F2")}\n";

				sb.Append(firstLine);
				sb.Append(secondLine);


				//Console.WriteLine("{0,3}) {1,-4} ({2})", i + 1, win ? "Win" : "Loss", champ.Name());

				//Console.WriteLine("     K/D/A {0}/{1}/{2} ({3:0.00})", k, d, a, kda);
			}

			return sb.ToString();
		}
    }
}

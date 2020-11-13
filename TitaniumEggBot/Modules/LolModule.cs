using Discord;
using Discord.Commands;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitaniumEggBot.LeagueOfLegends;

namespace TitaniumEggBot.Modules
{
    // Create a module with no prefix
    public class LolModule : ModuleBase<SocketCommandContext>
    {
        private readonly LolApiHelper _lolApiHelper;

        LolModule(LolApiHelper lolApiHelper)
        {
            _lolApiHelper = lolApiHelper;
        }

        /// <summary>
        /// Gets last 10 ARAM matches played by user
        /// </summary>
        /// <param name="summonerName"></param>
        /// <returns></returns>
        [Command("lolarams")]
        [Summary("Prints performance of player in arams.")]
        public async Task GetLolAramsAsync([Remainder][Summary("Player in-game name")] string summonerName)
        {
            Embed embed = await CreateMatchHistoryEmbed(summonerName, 450, Context);

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Gets last 10 Ranked flex matches played by user
        /// </summary>
        /// <param name="summonerName"></param>
        /// <returns></returns>
        [Command("lolflex")]
        [Summary("Prints performance of player in Ranked flex games.")]
        public async Task GetLolRankedFlexAsync([Remainder][Summary("Player in-game name")] string summonerName)
        {
            Embed embed = await CreateMatchHistoryEmbed(summonerName, 440, Context);

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Gets last 10 Ranked Solo/Duo matches played by user
        /// </summary>
        /// <param name="summonerName"></param>
        /// <returns></returns>
        [Command("lolrankedgames")]
        [Summary("Prints performance of player in last 10 ranked games.")]
        public async Task GetLolRankedGamesAsync([Remainder][Summary("Player in-game name")] string summonerName)
        {

            Embed embed = await CreateMatchHistoryEmbed(summonerName, 420, Context);

            await ReplyAsync(embed: embed);
        }

        /// <summary>
        /// Gets user's Ranked Solo/Duo Rank
        /// </summary>
        /// <param name="summonerName"></param>
        /// <returns></returns>
        [Command("lolrank")]
        [Summary("Prints player rank.")]
        public async Task GetLolRankAsync([Remainder][Summary("Player in-game name")] string summonerName)
        {
            string resultString = await _lolApiHelper.GetPlayerRankNAAsync(summonerName);

            await ReplyAsync(resultString);
        }
        // ReplyAsync is a method on ModuleBase 

        public async Task<Embed> CreateMatchHistoryEmbed(string summonerName, int queueType, SocketCommandContext context)
        {

            MatchHistory matchHistory = await _lolApiHelper.GetMatchHistoryAsync(summonerName, queueType);

            if (matchHistory is null)
            {
                return new EmbedBuilder
                {
                    Title = "Summoner name not found!"
                }.WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .Build();
            } else
            {
                if(matchHistory.Matches is null)
                {
                    return new EmbedBuilder
                    {
                        Title = $"No matches found for this queue type!"
                    }.WithColor(Color.Blue)
                    .WithCurrentTimestamp()
                    .Build();
                } else
                {

                    List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                    foreach(var matchStats in matchHistory.Matches)
                    {
                        string winOrLoss = matchStats.Win ? "Win" : "Loss";
                        string kda = LolApiHelper.CalculateKDA(matchStats.Kills, matchStats.Deaths, matchStats.Assists);

                        EmbedFieldBuilder embedField = new EmbedFieldBuilder
                        {
                            Name = $"{winOrLoss} on {matchStats.Champion}",
                            Value = $"K/D/A {matchStats.Kills}/{matchStats.Deaths}/{matchStats.Assists} - {kda}"
                        };
                        embedFields.Add(embedField);
                    }

                    var embedBuilder = new EmbedBuilder
                    {
                        Title = $"Match History",
                        Description = matchHistory.SummonerName,
                        Fields = embedFields
                    };

                    return embedBuilder.WithAuthor(context.Client.CurrentUser)
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp()
                        .Build();

                }
            }
        }
    }
}

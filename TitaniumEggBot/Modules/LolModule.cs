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
            string resultString = await _lolApiHelper.GetMatchHistoryStringAsync(summonerName, 450);

            await ReplyAsync(resultString);

            //await ReplyAsync(summonerData.Name);
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
            string resultString = await _lolApiHelper.GetMatchHistoryStringAsync(summonerName, 420);

            await ReplyAsync(resultString);

            //await ReplyAsync(summonerData.Name);
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
    }
}

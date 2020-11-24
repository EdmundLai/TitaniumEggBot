using AngleSharp;
using AngleSharp.Dom;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitaniumEggBot.Rotmg;

namespace TitaniumEggBot.Modules
{
    // Create a module with no prefix
    public class RotmgModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Getting detailed info about ROTMG player's characters
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [Command("realmplayer")]
        [Summary("Gets info about ROTMG player.")]
        public async Task GetInfoAsync([Remainder][Summary("player in-game name")] string player)
        {
            RotmgPlayer playerInfo = await RotmgApiHelper.GetCharacterInfoInListAsync(player);

            if(playerInfo is null)
            {
                await ReplyAsync("Player not found.");
            } else
            {
                List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                foreach (var character in playerInfo.Characters)
                {
                    EmbedFieldBuilder embedField = new EmbedFieldBuilder
                    {
                        Name = $"Ranked {character.RankedPlace.DisplayWithSuffix()} {character.Stats} {character.Class}",
                        Value = $"Weapon: {character.Equipment.Weapon}\nAbility: {character.Equipment.Ability}\nArmor: {character.Equipment.Armor}\nRing: {character.Equipment.Ring}"
                    };
                    embedFields.Add(embedField);
                }

                var embedBuilder = new EmbedBuilder
                {
                    Title = playerInfo.PlayerName,
                    Description = $"{RotmgApiHelper.GetStarRanking(playerInfo.Rank)} star with {playerInfo.Characters.Count} characters",
                    Fields = embedFields,
                    Url = playerInfo.RealmeyeURL
                };

                var embed = embedBuilder.WithAuthor(Context.Client.CurrentUser)
                    .WithCurrentTimestamp()
                    .Build();

                await ReplyAsync(embed: embed);
            }
        }

        // template for using embeds
        [Command("embed")]
        [Summary("command used for testing out how embeds work")]
        public async Task SendRichEmbedAsync()
        {
            var field0 = new EmbedFieldBuilder
            {
                Name = "test",
                Value = "haHAA"
            };
            var embedBuilder = new EmbedBuilder
            {
                // Embed property can be set within object initializer
                Title = "Hello world!",
                Description = "I am a description set by initializer.",
                Fields = new List<EmbedFieldBuilder> { field0 }
            };

            
            // Or with methods
            var embed = embedBuilder.AddField("Field title",
                "Field value. I also support [hyperlink markdown](https://example.com)!")
                .AddField("Field title",
                "Field value. I also support [hyperlink markdown](https://example.com)!")
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(footer => footer.Text = "I am a footer.")
                .WithColor(Color.Blue)
                .WithTitle("I overwrote \"Hello world!\"")
                .WithDescription("I am a description.")
                .WithUrl("https://example.com")
                .WithCurrentTimestamp()
                .Build();
            await ReplyAsync(embed: embed);
        }
        // ReplyAsync is a method on ModuleBase 
    }
}

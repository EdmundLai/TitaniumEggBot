using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;
using HattingtonGame;
using System.Linq;
using Discord;

namespace TitaniumEggBot.Modules
{
    public class HattingtonTestModule : ModuleBase<SocketCommandContext>
    {
        [Command("addtesthat")]
        [Summary("Add test pufferfish hat.")]
        public async Task AddTestHatAsync()
        {
            bool success = await HattingtonGameEngine.TestAddHatAsync();

            string successString = success ? "Hat added successfully!" : "Hat already exists!";

            await ReplyAsync(successString);
        }

        [Command("getfirsthat")]
        [Summary("Gets the first hat from database of all hats")]
        public async Task GetSomeHat()
        {
            var hat = HattingtonGameEngine.GetFirstHat();

            await ReplyAsync(hat.HatName);
        }

        [Command("randomforaged")]
        [Summary("Retrieves a random foraged item from the database")]
        public async Task GetRandomFoodAsync()
        {
            Food food = HattingtonGameEngine.GetRandomForagedFood();

            var embedFields = new List<EmbedFieldBuilder>();

            var mainField = new EmbedFieldBuilder
            {
                Name = food.Name,
                Value = $"{food.Energy} fullness restored"
            };

            embedFields.Add(mainField);

            var embed = HattingtonUtilities.CreateEmbed($"Random Foraged Food", embedFields, Context);

            await ReplyAsync(embed: embed);
        }
    }
}

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame;
using System.Linq;
using Discord;

namespace TitaniumEggBot.Modules
{
    public class HattingtonModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("gethats")]
        [Summary("Gets the hats in the database.")]
        public async Task GetHatsAsync()
        {
            using(var db = new Hattington())
            {
                IQueryable<Hat> hats = db.Hats;

                List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                foreach(Hat h in hats)
                {
                    EmbedFieldBuilder embedField = new EmbedFieldBuilder
                    {
                        Name = $"{h.HatID}) {h.HatName}",
                        Value = $"Tier {h.HatTierID} hat"
                    };
                    embedFields.Add(embedField);
                }

                Embed embed = CreateEmbed("Hat", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("gethattiers")]
        [Summary("Gets the hat tiers from the database.")]
        public async Task GetHatTiersAsync()
        {
            using(var db = new Hattington())
            {
                IQueryable<HatTier> hatTiers = db.HatTiers;

                List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                foreach(HatTier hatTier in hatTiers)
                {
                    var embedField = new EmbedFieldBuilder
                    {
                        Name = $"Tier {hatTier.HatTierID}",
                        Value = $"Levels {hatTier.MinLevel}-{hatTier.MaxLevel}"
                    };
                    embedFields.Add(embedField);
                }

                Embed embed = CreateEmbed("Hat Tiers", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("gethatchars")]
        [Summary("Gets the hat characters from the database.")]
        public async Task GetHatCharactersAsync()
        {
            using(var db = new Hattington())
            {
                var characters = db.HatCharacters;

                var embedFields = new List<EmbedFieldBuilder>();

                foreach(var character in characters)
                {
                    string fieldDescription = $"Level: {character.Level}\n" +
                        $"Hat: {db.Hats.Find(character.HatID).HatName}\n" +
                        $"Health: {character.Health}/{character.MaxHealth}\n" +
                        $"Attack: {character.Attack}\n" +
                        $"Defense: {character.Defense}\n" +
                        $"Magic: {character.Magic}\n" +
                        $"Magic Defense: {character.MagicDefense}\n" +
                        $"Stamina: {character.Stamina}/{character.MaxStamina}\n" +
                        $"Fullness: {character.Fullness}/{character.MaxFullness}";

                    var embedField = new EmbedFieldBuilder
                    {
                        Name = character.CharacterName,
                        Value = fieldDescription
                    };

                    embedFields.Add(embedField);
                }

                Embed embed = CreateEmbed("Hat Characters", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("addtesthat")]
        [Summary("Add test pufferfish hat.")]
        public async Task AddTestHatAsync()
        {
            bool success = await HattingtonGameEngine.TestAddHatAsync();

            string successString = success ? "Hat added successfully!" : "Hat already exists!";

            await ReplyAsync(successString);
        }

        [Command("addhatchar")]
        [Summary("Add character with provided name")]
        public async Task AddTestCharacterAsync(string name)
        {
            bool success = await HattingtonGameEngine.AddNewCharacterAsync(name);

            string successString = success ? "Character added successfully!" : "Character with that name already exists!";

            await ReplyAsync(successString);
        }

        // Small helper method to reduce redundant code
        private Embed CreateEmbed(string title, List<EmbedFieldBuilder> embedFields)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = title,
                Fields = embedFields
            };

            Embed embed = embedBuilder
                .WithAuthor(Context.Client.CurrentUser)
                .WithCurrentTimestamp()
                .Build();

            return embed;
        }
        // ReplyAsync is a method on ModuleBase 
    }
}

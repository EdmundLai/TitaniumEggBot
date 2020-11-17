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

        [Command("getenemies")]
        [Summary("Gets the hat enemies from the database.")]
        public async Task GetHatEnemiesAsync()
        {
            using(var db = new Hattington())
            {
                var enemies = db.Enemies;

                var embedFields = new List<EmbedFieldBuilder>();

                foreach(var enemy in enemies)
                {
                    string fieldDescription = $"Level: {enemy.Level}\n" +
                        $"Max Health: {enemy.MaxHealth}\n" +
                        $"Attack: {enemy.Attack}\n" +
                        $"Defense: {enemy.Defense}\n" +
                        $"Magic: {enemy.Magic}\n" +
                        $"Magic Defense: {enemy.MagicDefense}";

                    var embedField = new EmbedFieldBuilder
                    {
                        Name = enemy.Name,
                        Value = fieldDescription

                    };
                    embedFields.Add(embedField);
                }

                var embed = CreateEmbed("Enemies", embedFields);

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
        public async Task AddTestCharacterAsync(string name = "")
        {

            if(name.Length == 0)
            {
                await ReplyAsync("Please enter a name for your character.");
            } else
            {
                bool success = await HattingtonGameEngine.AddNewCharacterAsync(name, Context.User.ToString());

                string successString = success ? "Character added successfully!" : "User already created a character!";

                await ReplyAsync(successString);
            }
        }

        [Command("fight")]
        [Summary("Fight a random enemy from the pool of enemies")]
        public async Task FightEnemyAsync()
        {
            var log = await HattingtonGameEngine.FightEnemy(Context.User.ToString());

            if (!log.IsValid)
            {
                await ReplyAsync(log.Error);
            } else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                Console.WriteLine($"Number of events: {log.Events.Count}");

                for(int i = 0; i < log.Events.Count; i++)
                {
                    var fight = log.Events[i];

                    string description = log.PlayerFirst ? $"{fight.PlayerAction}\n{fight.EnemyAction}" : $"{fight.EnemyAction}\n{fight.PlayerAction}";

                    var embedField = new EmbedFieldBuilder
                    {
                        Name = $"Round {i+1}",
                        Value = description
                    };
                    embedFields.Add(embedField);
                }

                string battleResult = log.PlayerWin ? "Victory!" : "Defeat";

                var embedBuilder = new EmbedBuilder
                {
                    Title = battleResult,
                    Description = $"Battle between {log.CharacterName} and {log.EnemyName}",
                    Fields = embedFields
                };

                Embed embed = embedBuilder
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithCurrentTimestamp()
                    .Build();

                await ReplyAsync(embed: embed);
            }
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

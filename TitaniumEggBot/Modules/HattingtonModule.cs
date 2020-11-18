﻿using Discord.Commands;
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
            using (var db = new Hattington())
            {
                IQueryable<Hat> hats = db.Hats;

                List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                foreach (Hat h in hats)
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
            using (var db = new Hattington())
            {
                IQueryable<HatTier> hatTiers = db.HatTiers;

                List<EmbedFieldBuilder> embedFields = new List<EmbedFieldBuilder>();

                foreach (HatTier hatTier in hatTiers)
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

        [Command("chars")]
        [Summary("Gets the hat characters from the database.")]
        public async Task GetHatCharactersAsync()
        {
            using (var db = new Hattington())
            {
                var characters = db.HatCharacters;

                var embedFields = new List<EmbedFieldBuilder>();

                foreach (var character in characters)
                {
                    var embedField = new EmbedFieldBuilder
                    {
                        Name = character.CharacterName,
                        Value = GenerateCharacterDescription(character)
                    };

                    embedFields.Add(embedField);
                }

                Embed embed = CreateEmbed("Hat Characters", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("char")]
        [Summary("Get player's character")]
        public async Task GetCharacterAsync()
        {
            var character = HattingtonGameEngine.GetCharacter(Context.User.ToString());

            var embedFields = new List<EmbedFieldBuilder>();

            var embedField = new EmbedFieldBuilder
            {
                Name = character.CharacterName,
                Value = GenerateCharacterDescription(character)
            };

            embedFields.Add(embedField);

            Embed embed = CreateEmbed("Hat Character", embedFields);

            await ReplyAsync(embed: embed);
        }

        [Command("enemies")]
        [Summary("Gets the hat enemies from the database.")]
        public async Task GetHatEnemiesAsync()
        {
            using (var db = new Hattington())
            {
                var enemies = db.Enemies;

                var embedFields = new List<EmbedFieldBuilder>();

                foreach (var enemy in enemies)
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
        public async Task AddCharacterAsync(string name = "")
        {

            if (name.Length == 0)
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

                //Console.WriteLine($"Number of events: {log.Events.Count}");

                // may have to do a summary instead of each round
                // due to the amount of fields in the case of fights that take many rounds
                //for(int i = 0; i < log.Events.Count; i++)
                //{
                //    var fight = log.Events[i];

                //    string description = log.PlayerFirst ? $"{fight.PlayerAction}\n{fight.EnemyAction}" : $"{fight.EnemyAction}\n{fight.PlayerAction}";

                //    var embedField = new EmbedFieldBuilder
                //    {
                //        Name = $"Round {i+1}",
                //        Value = description
                //    };

                //    embedFields.Add(embedField);
                //}

                string lastStatement = log.PlayerWin ? $"{log.CharacterName} was victorious over a {log.EnemyName}!" : $"{log.CharacterName} was defeated by a {log.EnemyName}.";

                var concludingField = new EmbedFieldBuilder
                {
                    Name = lastStatement,
                    Value = $"{log.DamageTakenByPlayer} damage taken by {log.CharacterName}\n" +
                    $"{log.DamageTakenByEnemy} damage taken by {log.EnemyName}\n" +
                    $"{log.ExpGained} Exp Gained."
                };

                embedFields.Add(concludingField);

                string battleResult = log.PlayerWin ? "Victory!" : "Defeat";

                var embedBuilder = new EmbedBuilder
                {
                    Title = battleResult,
                    Fields = embedFields
                };

                Embed embed = embedBuilder
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithCurrentTimestamp()
                    .Build();

                await ReplyAsync(embed: embed);
            }
        }

        [Command("rest")]
        [Summary("Allows character to rest and regain stamina; reduces fullness by 8% of max fullness")]
        public async Task RestCharacterAsync()
        {
            var log = await HattingtonGameEngine.Rest(Context.User.ToString());

            if (!log.IsValid)
            {
                await ReplyAsync(log.Error);
            } else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                var mainField = new EmbedFieldBuilder
                {
                    Name = "Details",
                    Value = $"+{log.StaminaGained} stamina\n" +
                    $"-{log.FullnessCost} fullness"
                };

                embedFields.Add(mainField);

                var embed = CreateEmbed($"{log.CharacterName} is now rested!", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("heal")]
        [Summary("Allows character to heal 50% of max hp; costs 30 stamina")]
        public async Task HealCharacterAsync()
        {
            var log = await HattingtonGameEngine.Heal(Context.User.ToString());

            if (!log.IsValid)
            {
                await ReplyAsync(log.Error);
            }
            else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                var mainField = new EmbedFieldBuilder
                {
                    Name = "Details",
                    Value = $"+{log.HealthGained} health\n" +
                    $"-{log.StaminaCost} stamina"
                };

                embedFields.Add(mainField);

                var embed = CreateEmbed($"{log.CharacterName} has regained health!", embedFields);

                await ReplyAsync(embed: embed);
            }
        }

        // need to create module function for !eat to regain fullness


        // helper method for creating hat character embed
        public static string GenerateCharacterDescription(HatCharacter character)
        {
            return $"Level: {character.Level}\n" +
                        $"Experience: {character.Experience}\n" +
                        $"Hat: {HattingtonGameEngine.GetHatName(character.HatID)}\n" +
                        $"Health: {character.Health}/{character.MaxHealth}\n" +
                        $"Attack: {character.Attack}\n" +
                        $"Defense: {character.Defense}\n" +
                        $"Magic: {character.Magic}\n" +
                        $"Magic Defense: {character.MagicDefense}\n" +
                        $"Stamina: {character.Stamina}/{character.MaxStamina}\n" +
                        $"Fullness: {character.Fullness}/{character.MaxFullness}";
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

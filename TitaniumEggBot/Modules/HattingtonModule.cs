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

                Embed embed = HattingtonUtilities.CreateEmbed("Hat", embedFields, Context);

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

                Embed embed = HattingtonUtilities.CreateEmbed("Hat Tiers", embedFields, Context);

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
                        Value = HattingtonUtilities.GenerateCharacterDescription(character)
                    };

                    embedFields.Add(embedField);
                }

                Embed embed = HattingtonUtilities.CreateEmbed("Hat Characters", embedFields, Context);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("char")]
        [Summary("Get player's character")]
        public async Task GetCharacterAsync()
        {
            var character = HattingtonGameEngine.GetCharacter(Context.User.ToString());

            if (character == null)
            {
                await ReplyAsync("Character not found! Please add a new character using !addchar CharacterName");
                return;
            }

            var embedFields = new List<EmbedFieldBuilder>();

            var embedField = new EmbedFieldBuilder
            {
                Name = character.CharacterName,
                Value = HattingtonUtilities.GenerateCharacterDescription(character)
            };

            embedFields.Add(embedField);

            Embed embed = HattingtonUtilities.CreateEmbed("Hat Character", embedFields, Context);

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

                var embed = HattingtonUtilities.CreateEmbed("Enemies", embedFields, Context);

                await ReplyAsync(embed: embed);

            }
        }

        [Command("addchar")]
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

                var embed = HattingtonUtilities.CreateEmbed($"{log.CharacterName} is now rested!", embedFields, Context);

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

                var embed = HattingtonUtilities.CreateEmbed($"{log.CharacterName} has regained health!", embedFields, Context);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("forage")]
        [Summary("Forage for food to restore fullness, has 0 stamina cost")]
        public async Task ForageFoodAsync()
        {
            var forageLog = await HattingtonGameEngine.Forage(Context.User.ToString());

            if (!forageLog.IsValid)
            {
                await ReplyAsync(forageLog.Error);
            } else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                var mainField = new EmbedFieldBuilder
                {
                    Name = $"{forageLog.FoodName} found!",
                    Value = $"{forageLog.FoodCategory} Item\n" +
                    $"Restores {forageLog.Energy} fullness"
                };

                embedFields.Add(mainField);

                var embed = HattingtonUtilities.CreateEmbed("Foraging Results", embedFields, Context);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("hunt")]
        [Summary("Hunt for food to restore fullness, has 5 stamina cost")]
        public async Task HuntFoodAsync()
        {
            var huntLog = await HattingtonGameEngine.Hunt(Context.User.ToString());

            if (!huntLog.IsValid)
            {
                await ReplyAsync(huntLog.Error);
            }
            else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                var mainField = new EmbedFieldBuilder
                {
                    Name = $"{huntLog.FoodName} found!",
                    Value = $"{huntLog.FoodCategory} Item\n" +
                    $"Restores {huntLog.Energy} fullness"
                };

                embedFields.Add(mainField);

                var embed = HattingtonUtilities.CreateEmbed("Hunting Results", embedFields, Context);

                await ReplyAsync(embed: embed);
            }
        }

        [Command("inv")]
        [Summary("Get all food items from inventory")]
        public async Task GetInventoryAsync()
        {
            var inventoryLog = HattingtonGameEngine.GetInventory(Context.User.ToString());

            if (!inventoryLog.IsValid)
            {
                await ReplyAsync(inventoryLog.Error);
            } else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                foreach(FoodItem food in inventoryLog.Items)
                {
                    var embedField = new EmbedFieldBuilder
                    {
                        Name = $"{food.Food.Name} x {food.Quantity}",
                        Value = $"{food.Food.Energy} fullness restored"
                    };

                    embedFields.Add(embedField);
                }

                var embed = HattingtonUtilities.CreateEmbed("Inventory", embedFields, Context);

                await ReplyAsync(embed: embed);
            }

        }

        // need to create module function for !eat to regain fullness
        [Command("eat")]
        [Summary("Eat lowest energy food to restore fullness")]
        public async Task EatFoodAsync()
        {
            var eatLog = await HattingtonGameEngine.Eat(Context.User.ToString());

            if (!eatLog.IsValid)
            {
                await ReplyAsync(eatLog.Error);
            } else
            {
                var embedFields = new List<EmbedFieldBuilder>();

                string maxFullnessLine = eatLog.AtMaxFullness ? $"{eatLog.CharacterName} is now completely full." : "";

                var mainField = new EmbedFieldBuilder
                {
                    Name = $"{eatLog.CharacterName} ate a {eatLog.FoodName}!",
                    Value = $"{eatLog.CharacterName} now has {eatLog.CurrentFullness} fullness.\n" +
                    $"{maxFullnessLine}"
                };

                embedFields.Add(mainField);

                var embed = HattingtonUtilities.CreateEmbed("Meal time!", embedFields, Context);

                await ReplyAsync(embed: embed);
            }
        }


        // ReplyAsync is a method on ModuleBase 
    }
}

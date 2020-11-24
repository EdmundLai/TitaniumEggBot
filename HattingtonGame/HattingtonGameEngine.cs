using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;
using HattingtonGame.LogTypes;

namespace HattingtonGame
{
    public class HattingtonGameEngine
    {
        public static async Task<bool> TestAddHatAsync()
        {
            Hat h = new Hat
            {
                HatName = "Pufferfish",
                HatTierID = 1
            };

            return await HattingtonDbEditor.AddNewHat(h);
        }

        // adds character to database
        public static async Task<bool> AddNewCharacterAsync(string name, string discordUser)
        {
            var rand = new Random();

            List<int> hatIds = new List<int>();

            using (var db = new Hattington())
            {
                // hat tier 1 is the lowest tier
                hatIds = db.Hats.Where(h => h.HatTierID == 1)
                    .Select(h => h.HatID).ToList();
            }

            int randIndex = rand.Next(hatIds.Count);

            // create new character and add it to the db
            HatCharacter hatCharacter = new HatCharacter
            {
                CharacterName = name,
                DiscordUser = discordUser,
                HatID = hatIds[randIndex],
                Health = 100,
                MaxHealth = 100,
                Level = 1,
                Experience = 0,
                Attack = rand.Next(8, 11),
                Defense = rand.Next(5, 8),
                Magic = rand.Next(7, 10),
                MagicDefense = rand.Next(6, 9),
                Stamina = 100,
                MaxStamina = 100,
                Fullness = 100,
                MaxFullness = 100
            };

            bool justCreated = await HattingtonDbEditor.AddNewHatCharacter(hatCharacter);

            // create inventory for character if character has just been created
            if (justCreated)
            {
                // set initial values of HuntedItemsString and ForagedItemsString to empty strings
                Inventory charInventory = new Inventory
                {
                    DiscordUser = discordUser,
                    FoodItemsString = "",
                };

                await HattingtonDbEditor.AddNewCharacterInventoryAsync(charInventory);
            }

            return justCreated;
        }

        public static HatCharacter GetCharacter(string discordUser)
        {
            using (var db = new Hattington())
            {
                return HattingtonDbEditor.GetUserCharacter(discordUser, db);
            }
        }

        public static string GetHatName(int id)
        {
            return HattingtonDbEditor.GetHatNameFromId(id);
        }

        public static async Task<FightLog> FightEnemy(string discordUser)
        {
            return await HattingtonFightHandler.FightEnemy(discordUser);
        }

        // called by !rest in HattingtonModule
        public static async Task<RestLog> Rest(string discordUser)
        {
            return await HattingtonRestoreHandler.Rest(discordUser);
        }

        // called by !heal in HattingtonModule
        // restores 50% of health, consumes 30 stamina
        public static async Task<HealLog> Heal(string discordUser)
        {
            return await HattingtonRestoreHandler.Heal(discordUser);  
        }

        public static async Task<HuntForageLog> Forage(string discordUser)
        {
            return await HattingtonFoodHandler.Forage(discordUser);
        }

        public static async Task<HuntForageLog> Hunt(string discordUser)
        {
            return await HattingtonFoodHandler.Hunt(discordUser);
        }

        public static async Task<EatLog> Eat(string discordUser)
        {
            return await HattingtonFoodHandler.Eat(discordUser);
        }

        public static InventoryLog GetInventory(string discordUser)
        {
            return HattingtonFoodHandler.GetFoodInventory(discordUser);
        }

        public static Food GetRandomForagedFood()
        {
            return HattingtonFoodHandler.GetRandomForagedFood();
        }

        // test method
        public static Hat GetFirstHat()
        {
            using(var db = new Hattington())
            {
                return db.Hats.FirstOrDefault();
            }
        }
    }
}

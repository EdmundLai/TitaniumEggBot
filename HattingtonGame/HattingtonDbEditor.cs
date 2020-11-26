using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;


namespace HattingtonGame
{
    class HattingtonDbEditor
    {
        // need to add if character with character name already exists
        // returns true if character is added successfully
        public static async Task<bool> AddNewHatCharacter(HatCharacter character)
        {
            await using(var db = new Hattington())
            {
                //var existingChar = db.HatCharacters.Where(c => c.DiscordUser == character.DiscordUser).FirstOrDefault();
                if(!UserHasExistingCharacter(character.DiscordUser))
                {
                    db.HatCharacters.Add(character);
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
                
            }
        }

        public static bool UserHasExistingCharacter(string discordUser)
        { 
            using(var db = new Hattington())
            {
                return GetUserCharacter(discordUser, db) != null;
            }
        }

        public static async Task<bool> LevelUpCharacter(string discordUser, Hattington db)
        {
            // if there is no character, don't level up and just return

            var character = GetUserCharacter(discordUser, db);

            if(character != null)
            {
                Random rand = new Random();

                int healthIncrease = rand.Next(5, 11);

                character.Level += 1;
                character.Health += healthIncrease;
                character.MaxHealth += healthIncrease;
                character.Attack += rand.Next(1, 3);
                character.Defense += rand.Next(1, 3);
                character.Magic += rand.Next(1, 3);
                character.MagicDefense += rand.Next(1, 3);
                character.MaxStamina += 5;

                await db.SaveChangesAsync();

                return true;
            }

            return false;

        }

        public static async Task<bool> DeductFromCharacterStamina(string discordUser, int staminaCost, Hattington db)
        {
            var character = GetUserCharacter(discordUser, db);

            if (character != null)
            {
                character.Stamina -= staminaCost;

                await db.SaveChangesAsync();

                return true;
            }

            return false;

        }

        public static HatCharacter GetUserCharacter(string discordUser, Hattington db)
        {
            return db.HatCharacters.Where(c => c.DiscordUser == discordUser).FirstOrDefault();
        }

        public static string GetUserCharacterName(string discordUser, Hattington db)
        {
            return GetUserCharacter(discordUser, db).CharacterName;
        }

        public static Inventory GetPlayerInventory(string discordUser, Hattington db)
        {
            return db.CharacterInventories.Where(inv => inv.DiscordUser == discordUser).FirstOrDefault();
        }

        public static string GetHatNameFromId(int id)
        {
            using(var db = new Hattington())
            {
                return db.Hats.Find(id).HatName;
            }
        }

        public static List<HatCharacter> GetCharacters()
        {
            using(var db = new Hattington())
            {
                return db.HatCharacters.ToList();
            }
        }

        public static string GetFoodCategory(int itemTypeID, Hattington db)
        {
            string foodCategory = db.ItemTypes
                    .Where(type => type.ItemTypeID == itemTypeID)
                    .Select(type => type.TypeName)
                    .FirstOrDefault();

            return foodCategory;
        }

        public static Food GetFoodFromId(int itemID, Hattington db)
        {
            return db.FoodItems.Where(food => food.ItemID == itemID).FirstOrDefault();
        }

        public static async Task<bool> AddNewHat(Hat hat)
        {
            await using (var db = new Hattington())
            {
                var existingHat = db.Hats.Where(h => h.HatName == hat.HatName).FirstOrDefault();
                if(existingHat is null)
                {
                    db.Hats.Add(hat);
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }

        // this method should always be called when character is created, since it overwrites existing inventory
        // otherwise, when looking for existing inventory, when trying to add to inventory,
        // if no inventory exists matching the discord user, send a message that player needs to create a character first
        //
        // not sure how to do error handling on these function calls if db fails
        public static async Task AddNewCharacterInventoryAsync(Inventory newInventory)
        {
            await using (var db = new Hattington())
            {
                var existingInventory = db.CharacterInventories.Where(inv => inv.DiscordUser == newInventory.DiscordUser).FirstOrDefault();

                if(existingInventory != null)
                {
                    db.Remove(existingInventory);
                }

                db.CharacterInventories.Add(newInventory);
                await db.SaveChangesAsync();
            }
        }
    }
}

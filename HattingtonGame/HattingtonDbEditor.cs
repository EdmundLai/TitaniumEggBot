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
            return GetUserCharacter(discordUser) != null;
        }

        public static HatCharacter GetUserCharacter(string discordUser)
        {
            using(var db = new Hattington())
            {
                return db.HatCharacters.Where(c => c.DiscordUser == discordUser).FirstOrDefault();
            }
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
    }
}

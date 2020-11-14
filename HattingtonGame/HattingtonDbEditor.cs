using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HattingtonGame
{
    class HattingtonDbEditor
    {
        // need to add if character with character name already exists
        public static async Task<bool> AddNewHatCharacter(HatCharacter character)
        {
            await using(var db = new Hattington())
            {
                var existingChar = db.HatCharacters.Where(c => c.CharacterName == character.CharacterName).FirstOrDefault();
                if(existingChar is null)
                {
                    db.HatCharacters.Add(character);
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
                
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

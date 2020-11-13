using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HattingtonGame
{
    class HattingtonDbEditor
    {
        public static async Task AddNewHatCharacter(HatCharacter character)
        {
            await using(var db = new Hattington())
            {
                db.HatCharacters.Add(character);
                await db.SaveChangesAsync();
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

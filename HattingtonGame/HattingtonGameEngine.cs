using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task<bool> AddNewCharacterAsync(string name)
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

            HatCharacter hatCharacter = new HatCharacter
            {
                CharacterName = name,
                HatID = hatIds[randIndex],
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

            return await HattingtonDbEditor.AddNewHatCharacter(hatCharacter);
        }
    }
}

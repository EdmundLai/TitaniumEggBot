using System;
using System.Collections.Generic;
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
    }
}

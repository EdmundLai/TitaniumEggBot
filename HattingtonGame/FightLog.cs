using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame
{
    public class FightLog
    {
        // If the fight did not take place, Fight log is not valid (reasons: user does not have character, character hp is already 0)
        public bool IsValid { get; set; }

        // Error is set if IsValid if false (Tells reason why fight is not valid)
        public string Error { get; set; }

        public List<FightEvent> Events { get; set; }

        public bool PlayerWin { get; set; }

        // true if player goes first in a fight
        public bool PlayerFirst { get; set; }

        public int PlayerEndHealth { get; set; }

        public string CharacterName { get; set; }

        public string EnemyName { get; set; }
    }
}

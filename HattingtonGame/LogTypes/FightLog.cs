using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class FightLog : EngineLog
    {
        public List<FightEvent> Events { get; set; }

        public bool PlayerWin { get; set; }

        // true if player goes first in a fight
        public bool PlayerFirst { get; set; }

        public int PlayerEndHealth { get; set; }

        public string EnemyName { get; set; }

        public int ExpGained { get; set; }

        public int DamageTakenByPlayer { get; set; }

        public int DamageTakenByEnemy { get; set; }

        public bool CharacterLeveledUp { get; set; }
    }
}

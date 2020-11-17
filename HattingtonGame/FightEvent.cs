using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame
{
    // One event occuring in a fight
    public class FightEvent
    {
        public string PlayerAction { get; set; }

        public string EnemyAction { get; set; }
    }
}

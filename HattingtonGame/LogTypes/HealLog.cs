using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class HealLog : EngineLog
    {
        public int HealthGained { get; set; }

        public int StaminaCost { get; set; }
    }
}

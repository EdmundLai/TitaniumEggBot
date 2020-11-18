using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class RestLog : EngineLog
    {
        public int StaminaGained { get; set; }

        public int FullnessCost { get; set; }
    }
}

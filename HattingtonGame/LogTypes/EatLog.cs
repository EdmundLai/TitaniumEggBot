using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class EatLog : EngineLog
    {
        public int FullnessRestored { get; set; }

        public string FoodName { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class HuntForageLog : EngineLog
    {
        public string FoodName { get; set; }

        public int Energy { get; set; }

        public string FoodCategory { get; set; }

    }
}

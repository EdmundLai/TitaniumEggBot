using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame.LogTypes
{
    public class InventoryLog : EngineLog
    {
        public List<FoodItem> Items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame
{
    // allows HattingtonFoodHandler to use RNG to pick a food
    public class FoodPickRange
    {
        public int ItemID { get; set; }

        public string Name { get; set; }

        public int Energy { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}

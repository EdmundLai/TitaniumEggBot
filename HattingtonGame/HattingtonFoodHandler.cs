using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;
using HattingtonGame.LogTypes;
using Microsoft.EntityFrameworkCore;

namespace HattingtonGame
{
    class HattingtonFoodHandler
    {
        // DO FIRST
        // (check if character is created before picking random food)
        // picks random food from Foraged Items in FoodItems table based on appearanceChance
        // add food to character's inventory 
        // TODO: Test this in HattingtonModule
        public static async Task<HuntForageLog> Forage(string discordUser)
        {
            return await GatherFood(discordUser, 1);
        }

        // DO SECOND
        // (check if character is created before picking random food)
        // picks random food from Hunted Items in FoodItems table based on appearanceChance
        // add food to character's inventory (check if character is created before picking random food)
        // TODO: Test this in HattingtonModule
        public static async Task<HuntForageLog> Hunt(string discordUser)
        {
            return await GatherFood(discordUser, 2);
        }

        // called by either Hunt or Forage (foodTypeID = 1 for forage, foodTypeID = 2 for hunt)
        public static async Task<HuntForageLog> GatherFood(string discordUser, int foodTypeID)
        {
            await using (var db = new Hattington())
            {
                var character = HattingtonDbEditor.GetUserCharacter(discordUser, db);

                int huntTypeID = 2;

                if (character == null)
                {
                    // do something
                    return new HuntForageLog
                    {
                        IsValid = false,
                        Error = "Character does not exist! Create a character using !addchar characterName"
                    };
                }

                if (character.Health == 0)
                {
                    return new HuntForageLog
                    {
                        IsValid = false,
                        Error = "Character cannot gather food on 0 health! Restore your health using !heal"
                    };
                }

                int staminaCost = 5;

                if (foodTypeID == huntTypeID)
                {
                    if (character.Stamina < staminaCost)
                    {
                        return new HuntForageLog
                        {
                            IsValid = false,
                            Error = "Character does not have enough stamina! Recharage your stamina using !rest"
                        };
                    }
                }
            
                var randomFood = GetRandomFoodFromTable(db.FoodItems, foodTypeID);

                var playerInventory = HattingtonDbEditor.GetPlayerInventory(discordUser, db);

                if (playerInventory == null)
                {
                    // should never happen, since character inventory is created at the same time that character is created
                    // and earlier check occurred for no character
                    return new HuntForageLog
                    {
                        IsValid = false,
                        Error = "Character inventory does not exist! Create a character using !addchar characterName"
                    };
                }

                playerInventory.FoodItemsString = GetNewFoodItemsStringAfterAdd(playerInventory, randomFood);

                await db.SaveChangesAsync();

                string foodCategory = HattingtonDbEditor.GetFoodCategory(randomFood.ItemTypeID, db);

                if(foodTypeID == huntTypeID)
                {
                    await HattingtonDbEditor.DeductFromCharacterStamina(discordUser, staminaCost, db);
                }

                return new HuntForageLog
                {
                    IsValid = true,
                    Character = character,
                    FoodCategory = foodCategory,
                    FoodName = randomFood.Name,
                    Energy = randomFood.Energy,
                    StaminaCost = staminaCost,
                };
            }
        }

        static string GetNewFoodItemsStringAfterAdd(Inventory inventory, Food food)
        {
            var foodItems = new List<string>();

            // string split works only if there are items in the list already, not an empty list
            if(inventory.FoodItemsString.Length > 0)
            {
                foodItems = inventory.FoodItemsString.Split(",").ToList();
            }

            foodItems.Add(food.ItemID.ToString());

            return string.Join(",", foodItems);
        }

        static string GetNewFoodItemsStringAfterRemove(Inventory inventory, Food food)
        {
            var foodItems = new List<string>();

            // string split works only if there are items in the list already, not an empty list
            if (inventory.FoodItemsString.Length > 0)
            {
                foodItems = inventory.FoodItemsString.Split(",").ToList();
            }

            foodItems.Remove(food.ItemID.ToString());

            return string.Join(",", foodItems);
        }

        // method used for testing
        public static Food GetRandomForagedFood()
        {
            using (var db = new Hattington())
            {
                return GetRandomFoodFromTable(db.FoodItems, 1);
            }
        }

        // method used for testing (has not been used in HattingtonModule yet)
        public static Food GetRandomHuntedFood()
        {
            using (var db = new Hattington())
            {
                return GetRandomFoodFromTable(db.FoodItems, 2);
            }
        }

        // how to pick random food
        // get all foods from table (foraged items or hunted items)
        // assign each an integer range based on their number of chances (appearanceChance)
        // choose random number with max being sum of all appearanceChances
        // find which food item range contains random number
        // return food item
        public static Food GetRandomFoodFromTable(DbSet<Food> table, int itemTypeId)
        {
            // get all food
            var foodList = table.Where(food => food.ItemTypeID == itemTypeId).ToList();

            //Console.WriteLine(foodList.Count);

            int currSumOfChances = 0;

            List<FoodPickRange> foodRangeList = new List<FoodPickRange>();

            foreach(var food in foodList)
            {
                int newSumOfChances = currSumOfChances + food.AppearanceChance;
                // valid range is minValue (inclusive) to maxValue (exclusive)
                var FoodRange = new FoodPickRange
                {
                    ItemID = food.ItemID,
                    Name = food.Name,
                    Energy = food.Energy,
                    MinValue = currSumOfChances,
                    MaxValue = newSumOfChances,
                };

                currSumOfChances = newSumOfChances;

                foodRangeList.Add(FoodRange);
            }

            Random rand = new Random();

            int chosenFoodNumber = rand.Next(currSumOfChances);

            // index of chosenFood
            FoodPickRange chosenFood = null;

            foreach(var foodRange in foodRangeList)
            {
                // stop iterating after chosenFood has been found
                if(chosenFoodNumber >= foodRange.MinValue && chosenFoodNumber < foodRange.MaxValue)
                {
                    chosenFood = foodRange;
                    break;
                }
            }

            if(chosenFood == null)
            {
                Console.WriteLine("chosen food is null. This should not happen if the random food picker is coded correctly.");
                return null;
            }

            // can return null if the food is not found in the table
            return table.Where(f => f.ItemID == chosenFood.ItemID).FirstOrDefault();
        }

        // DO THIRD
        // Get user's food in from inventory
        public static InventoryLog GetFoodInventory(string discordUser)
        {
            using(var db = new Hattington())
            {
                var foodItems = GetFoodItemsFromInventory(discordUser, db);

                var character = HattingtonDbEditor.GetUserCharacter(discordUser, db);

                if(foodItems == null)
                {
                    return new InventoryLog
                    {
                        IsValid = false,
                        Error = "Character does not exist! Create a character using !addchar characterName"
                    };
                }

                // handle case of empty string
                if(foodItems.Count == 0)
                {
                    return new InventoryLog
                    {
                        IsValid = false,
                        Error = "Player has no food items! Get more by using !forage or !hunt"
                    };
                }

                return new InventoryLog
                {
                    IsValid = true,
                    Character = character,
                    Items = foodItems

                };
            }
        }

        
        public static List<FoodItem> GetFoodItemsFromInventory(string discordUser, Hattington db = null)
        {
            if(db == null)
            {
                db = new Hattington();
            }

            Inventory playerInventory = HattingtonDbEditor.GetPlayerInventory(discordUser, db);

            if (playerInventory == null)
            {
                return null;
            }

            if (playerInventory.FoodItemsString.Length == 0)
            {
                // empty list
                return new List<FoodItem>();
            }

            var foodItems = playerInventory.FoodItemsString.Split(",").ToList();

            Dictionary<int, int> foodItemsDict = new Dictionary<int, int>();

            foreach (string foodID in foodItems)
            {
                //Console.WriteLine(foodID);
                int foodIntID = int.Parse(foodID);

                if (!foodItemsDict.ContainsKey(foodIntID))
                {
                    foodItemsDict.Add(foodIntID, 1);
                }
                else
                {
                    foodItemsDict[foodIntID] += 1;
                }
            }

            var foodList = new List<FoodItem>();

            foreach (var foodItemKVP in foodItemsDict)
            {
                var item = new FoodItem
                {
                    Food = HattingtonDbEditor.GetFoodFromId(foodItemKVP.Key, db),
                    Quantity = foodItemKVP.Value
                };
                foodList.Add(item);
            }

            return foodList;
            
        }

        // DO LAST
        // automatically choose a food for character to eat
        // try to pick the most optimal food
        // how to pick optimal food
        // prioritize lowest energy food
        public static async Task<EatLog> Eat(string discordUser)
        {
            using (var db = new Hattington())
            {
                var character =  HattingtonDbEditor.GetUserCharacter(discordUser, db);

                // character doesn't exist
                if(character == null)
                {
                    return new EatLog
                    {
                        IsValid = false,
                        Error = "Character does not exist! Create a character using !addchar characterName"
                    };
                }

                // character already full
                if(character.Fullness == character.MaxFullness)
                {
                    return new EatLog
                    {
                        IsValid = false,
                        Error = "Character is already full!"
                    };
                }

                var foodItems = GetFoodItemsFromInventory(discordUser, db);

                // no food to eat
                if(foodItems.Count == 0)
                {
                    return new EatLog
                    {
                        IsValid = false,
                        Error = "There is no food in the inventory! Get more food by using !forage or !hunt."
                    };
                }

                FoodItem selectedFood = GetLowestEnergyFood(foodItems);

                int originalFullness = character.Fullness;

                var playerInventory = HattingtonDbEditor.GetPlayerInventory(discordUser, db);

                playerInventory.FoodItemsString = GetNewFoodItemsStringAfterRemove(playerInventory, selectedFood.Food);

                character.Fullness = Math.Min(character.Fullness + selectedFood.Food.Energy, character.MaxFullness);

                await db.SaveChangesAsync();

                return new EatLog
                {
                    IsValid = true,
                    FullnessRestored = character.Fullness - originalFullness,
                    FoodName = selectedFood.Food.Name,
                    Character = character
                };
            }
        }

        public static FoodItem GetLowestEnergyFood(List<FoodItem> foodItems)
        {
            // find food with lowest energy and return it
            int lowestEnergy = 9999;

            FoodItem selectedFood = null;

            foreach (var item in foodItems)
            {
                if (item.Food.Energy < lowestEnergy)
                {
                    lowestEnergy = item.Food.Energy;
                    selectedFood = item;
                }
            }

            return selectedFood;
        }
    }
}

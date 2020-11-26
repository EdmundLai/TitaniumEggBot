using HattingtonGame.LogTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HattingtonGame
{
    class HattingtonRestoreHandler
    {
        // rest restores 50% of max stamina, cost 8% of max fullness
        // called by !rest in HattingtonModule
        public static async Task<RestLog> Rest(string discordUser)
        {
            await using (var db = new Hattington())
            {
                var character = HattingtonDbEditor.GetUserCharacter(discordUser, db);

                double scaleFactor = 0.5;

                double fullnessPercentage = 0.08;

                if (character == null)
                {
                    // character not found
                    return new RestLog
                    {
                        IsValid = false,
                        Error = "Character not found! Please add a new character using !addchar CharacterName",
                    };
                }

                if (character.Fullness <= 0.1 * character.MaxFullness)
                {
                    // character must be at least 10% full to rest
                    return new RestLog
                    {
                        IsValid = false,
                        Error = "Character must be at least at 10% fullness to rest! Use !eat to regain fullness!",
                    };
                }

                if(character.Stamina == character.MaxStamina)
                {
                    return new RestLog
                    {
                        IsValid = false,
                        Error = "Character already has max stamina!",
                    };
                }

                int originalStamina = character.Stamina;
                int originalFullness = character.Fullness;

                character.Stamina = CalculateNewValueAfterRestore(originalStamina, character.MaxStamina, scaleFactor);

                // resting costs 8% of max fullness
                character.Fullness -= (int)Math.Ceiling(fullnessPercentage * character.MaxFullness);

                await db.SaveChangesAsync();

                return new RestLog
                {
                    IsValid = true,
                    Character = character,
                    StaminaGained = character.Stamina - originalStamina,
                    FullnessCost = originalFullness - character.Fullness,
                };
            }
        }

        // called by !heal in HattingtonModule
        // restores 50% of health, consumes 30 stamina
        public static async Task<HealLog> Heal(string discordUser)
        {
            await using (var db = new Hattington())
            {
                var character = HattingtonDbEditor.GetUserCharacter(discordUser, db);

                int staminaCost = 30;

                double scaleFactor = 0.5;

                if (character == null)
                {
                    // character not found
                    return new HealLog
                    {
                        IsValid = false,
                        Error = "Character not found! Please add a new character using !addchar CharacterName",
                    };
                }

                if (character.Stamina < staminaCost)
                {
                    return new HealLog
                    {
                        IsValid = false,
                        Error = "Not enough stamina! Use !rest to regain stamina."
                    };
                }

                if(character.Health == character.MaxHealth)
                {
                    return new HealLog
                    {
                        IsValid = false,
                        Error = "Character health is already at max."
                    };
                }

                int originalHealth = character.Health;

                character.Health = CalculateNewValueAfterRestore(originalHealth, character.MaxHealth, scaleFactor);

                character.Stamina -= staminaCost;

                await db.SaveChangesAsync();

                return new HealLog
                {
                    IsValid = true,
                    Character = character,
                    HealthGained = character.Health - originalHealth,
                    StaminaCost = staminaCost,
                };
            }
        }


        // scale factor provides amount restored (i.e. 0.5 -> 50% of max value restored)
        static int CalculateNewValueAfterRestore(int originalValue, int maxValue, double scaleFactor)
        {
            int newValue = (int)Math.Round(originalValue + scaleFactor * maxValue);

            // capping newHealth at maxValue
            return Math.Min(newValue, maxValue);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;
using HattingtonGame.LogTypes;

namespace HattingtonGame
{
    public class HattingtonGameEngine
    {
        public static async Task<bool> TestAddHatAsync()
        {
            Hat h = new Hat
            {
                HatName = "Pufferfish",
                HatTierID = 1
            };

            return await HattingtonDbEditor.AddNewHat(h);
        }

        // adds character to database
        public static async Task<bool> AddNewCharacterAsync(string name, string discordUser)
        {
            var rand = new Random();

            List<int> hatIds = new List<int>();

            using (var db = new Hattington())
            {
                // hat tier 1 is the lowest tier
                hatIds = db.Hats.Where(h => h.HatTierID == 1)
                    .Select(h => h.HatID).ToList();
            }

            int randIndex = rand.Next(hatIds.Count);

            HatCharacter hatCharacter = new HatCharacter
            {
                CharacterName = name,
                DiscordUser = discordUser,
                HatID = hatIds[randIndex],
                Health = 100,
                MaxHealth = 100,
                Level = 1,
                Experience = 0,
                Attack = rand.Next(8, 11),
                Defense = rand.Next(5, 8),
                Magic = rand.Next(7, 10),
                MagicDefense = rand.Next(6, 9),
                Stamina = 100,
                MaxStamina = 100,
                Fullness = 100,
                MaxFullness = 100
            };

            return await HattingtonDbEditor.AddNewHatCharacter(hatCharacter);
        }

        public static HatCharacter GetCharacter(string discordUser)
        {
            using (var db = new Hattington())
            {
                return HattingtonDbEditor.GetUserCharacter(discordUser, db);
            }
        }

        public static string GetHatName(int id)
        {
            return HattingtonDbEditor.GetHatNameFromId(id);
        }

        // called by !fight being entered in the discord chat
        // fighting should also cost stamina
        // 10 stamina per fight
        // DONE: add experience mechanic if player won battle
        // DONE: add !rest function to allow player to regain hp
        // DONE: use stamina somehow
        public static async Task<FightLog> FightEnemy(string discordUser)
        {
            await using (var db = new Hattington())
            {

                var character = HattingtonDbEditor.GetUserCharacter(discordUser, db);

                if (character == null)
                {
                    return new FightLog
                    {
                        IsValid = false,
                        Error = "Character does not exist! Create a character using !addhatchar characterName"
                    };
                }

                // check if character has enough health to fight
                if (character.Health == 0)
                {
                    // need to implement !heal
                    return new FightLog
                    {
                        IsValid = false,
                        Error = "Character cannot fight at 0 hp! Use !heal to regain hp!"
                    };
                }

                // check if character has enough stamina to fight
                if(character.Stamina < 10)
                {
                    return new FightLog
                    {
                        IsValid = false,
                        Error = "Character needs 10 stamina to fight! Use !rest to restore stamina"
                    };
                }

                // Step 1: Get enemy from the corresponding pool
                Enemy chosenEnemy = GetChosenEnemy(character);

                // Step 2: initiate fight sequence

                // engine will set hp of player to new value if needed

                // don't need to set hp of enemy because new enemy will be fought every single time

                var rand = new Random();

                // initializing fight variables
                int playerCurrHealth = character.Health;
                int enemyCurrHealth = chosenEnemy.MaxHealth;

                bool fightNotOver = playerCurrHealth > 0 || enemyCurrHealth > 0;

                // fight initiative
                bool playerInitiative = character.Level >= chosenEnemy.Level;
                
                List<FightEvent> fightEvents = new List<FightEvent>();

                bool playerWin = false;

                int minDamage = 1;

                // bug: if damage is 0 on both parties, then fight will never end. This will happen if defense is higher than attack
                // can fix by forcing damage to be 1 at least, but that may not be the best solution
                while (fightNotOver)
                {
                    double enemyPhysAttackChance = (double)chosenEnemy.NormalAttackChances / (chosenEnemy.NormalAttackChances + chosenEnemy.MagicAttackChances);

                    double playerPhysAttackChance = (double)character.Attack / (character.Attack + character.Magic);
                    //double playerMagicAttackChance = 1 - playerPhysAttackChance;

                    int CalculateDamageVariance(int attackingLevel, int defendingLevel)
                    {
                        int levelDifference = attackingLevel - defendingLevel;

                        bool diffIsNegative = levelDifference < 0;

                        if (diffIsNegative)
                        {
                            levelDifference = Math.Abs(levelDifference);
                        }
                        
                        if(levelDifference == 0)
                        {
                            return 0;
                        }

                        int signMultiplier = diffIsNegative ? -1 : 1;

                        int damageVariance = (int)Math.Floor(0.5 * rand.Next(levelDifference)) * signMultiplier;

                        return damageVariance;
                    }

                    // return false if fight is over
                    string ExecutePlayerTurn()
                    {
                        // introduce some rng to damage dealt if there is level difference
                        int damageVariance = CalculateDamageVariance(character.Level, chosenEnemy.Level);

                        // roll type of attack
                        double attackTypeRoll = rand.NextDouble();

                        bool isPhysAttack = playerPhysAttackChance > attackTypeRoll;

                        // make sure damage cannot be negative
                        int damage = Math.Max(isPhysAttack ? character.Attack + damageVariance - chosenEnemy.Defense : character.Magic + damageVariance - chosenEnemy.MagicDefense, minDamage);

                        enemyCurrHealth -= damage;

                        enemyCurrHealth = Math.Max(enemyCurrHealth, 0);

                        string physOrMagic = isPhysAttack ? "Physical" : "Magic";

                        return $"{character.CharacterName} used a {physOrMagic} attack on the enemy for {damage} damage!";
                    }

                    string ExecuteEnemyTurn()
                    {
                        // introduce some rng to damage dealt if there is level difference
                        int damageVariance = CalculateDamageVariance(chosenEnemy.Level, character.Level);

                        // roll type of attack
                        double attackTypeRoll = rand.NextDouble();

                        bool isPhysAttack = enemyPhysAttackChance > attackTypeRoll;

                        // make sure damage cannot be negative
                        int damage = Math.Max(isPhysAttack ? chosenEnemy.Attack + damageVariance - character.Defense : chosenEnemy.Magic + damageVariance - character.MagicDefense, minDamage);

                        playerCurrHealth -= damage;

                        // curr health cannot be negative
                        playerCurrHealth = Math.Max(playerCurrHealth, 0);

                        string physOrMagic = isPhysAttack ? "Physical" : "Magic";

                        return $"{chosenEnemy.Name} used a {physOrMagic} attack on the player for {damage} damage!";
                    }

                    //higher level opponent attacks first, equal level player and enemy gives priority to player
                    if (playerInitiative)
                    {
                        var fightEvent = new FightEvent();
                        fightEvent.PlayerAction = ExecutePlayerTurn();
                        // if enemy gets killed, fight is over
                        if (enemyCurrHealth == 0)
                        {
                            fightEvents.Add(fightEvent);
                            playerWin = true;
                            break;
                        }
                        fightEvent.EnemyAction = ExecuteEnemyTurn();
                        fightEvents.Add(fightEvent);
                        if (playerCurrHealth == 0)
                        {
                            playerWin = false;
                            break;
                        }
                    } else
                    {
                        var fightEvent = new FightEvent();
                        fightEvent.EnemyAction = ExecuteEnemyTurn();
                        if (playerCurrHealth == 0)
                        {
                            fightEvents.Add(fightEvent);
                            playerWin = false;
                            break;
                        }
                        fightEvent.PlayerAction = ExecutePlayerTurn();
                        fightEvents.Add(fightEvent);
                        // if enemy gets killed, fight is over
                        if (enemyCurrHealth == 0)
                        {
                            playerWin = true;
                            break;
                        }
                    }
                }

                int damageTakenByPlayer = character.Health - playerCurrHealth;
                int damageTakenByEnemy = chosenEnemy.MaxHealth - enemyCurrHealth;

                // save player health to db
                character.Health = playerCurrHealth;

                // deduct stamina cost from player's stamina
                character.Stamina -= 10;

                // need to calculate/factor in experience gain
                // exp gain should be a db column
                int expGained = 0;

                bool characterLeveledUp = false;

                if (playerWin)
                {
                    expGained = chosenEnemy.ExpGain;

                    int newExperience = character.Experience + chosenEnemy.ExpGain;
                    //Console.WriteLine(chosenEnemy.ExpGain);

                    int expNeededForLevelUp = 50 + 50 * (character.Level - 1);

                    // need to level up stats with level up
                    // right now just gaining levels without any increase in stats
                    if(newExperience >= expNeededForLevelUp)
                    {
                        characterLeveledUp = true;
                        int healthIncrease = rand.Next(5, 11);

                        character.Level += 1;
                        character.Health += healthIncrease;
                        character.MaxHealth += healthIncrease;
                        character.Attack += rand.Next(1, 3);
                        character.Defense += rand.Next(1, 3);
                        character.Magic += rand.Next(1, 3);
                        character.MagicDefense += rand.Next(1, 3);
                        character.MaxStamina += 5;
                        character.Experience = newExperience - expNeededForLevelUp;
                    } else
                    {
                        character.Experience = newExperience;
                    }
                }

                await db.SaveChangesAsync();

                return new FightLog
                {
                    IsValid = true,
                    Events = fightEvents,
                    PlayerEndHealth = playerCurrHealth,
                    PlayerFirst = playerInitiative,
                    PlayerWin = playerWin,
                    CharacterName = character.CharacterName,
                    EnemyName = chosenEnemy.Name,
                    ExpGained = expGained,
                    DamageTakenByPlayer = damageTakenByPlayer,
                    DamageTakenByEnemy = damageTakenByEnemy,
                    CharacterLeveledUp = characterLeveledUp,
                };
            }
        }

        // rest restores 50% of max stamina, cost 8% of max fullness
        // called by !rest in HattingtonModule
        public static async Task<RestLog> Rest(string discordUser)
        {
            await using(var db = new Hattington())
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
                        Error = "Character not found! Please add a new character using !addhatchar CharacterName",
                    };
                }

                if(character.Fullness <= 0.1 * character.MaxFullness)
                {
                    // character must be at least 10% full to rest
                    return new RestLog
                    {
                        IsValid = false,
                        Error = "Character must be at least at 10% fullness to rest! Use !eat to regain fullness!",
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
                    CharacterName = character.CharacterName,
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
                        Error = "Character not found! Please add a new character using !addhatchar CharacterName",
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

                int originalHealth = character.Health;

                character.Health = CalculateNewValueAfterRestore(originalHealth, character.MaxHealth, scaleFactor);

                character.Stamina -= staminaCost;

                await db.SaveChangesAsync();

                return new HealLog
                {
                    IsValid = true,
                    CharacterName = character.CharacterName,
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

        static Enemy GetChosenEnemy(HatCharacter character)
        {
            using(var db = new Hattington())
            {
                // choose random enemy of appropriate rank
                var rank = db.EnemyRanks.Where(r => (character.Level >= r.MinLevel && character.Level <= r.MaxLevel)).FirstOrDefault();

                List<Enemy> enemies = db.Enemies.Where(e => e.EnemyRankID == rank.EnemyRankID).ToList();

                var rand = new Random();

                var randIndex = rand.Next(enemies.Count);

                return enemies[randIndex];
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;
using HattingtonGame.LogTypes;

namespace HattingtonGame
{
    class HattingtonFightHandler
    {
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
                        Error = "Character does not exist! Create a character using !addchar characterName"
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
                if (character.Stamina < 10)
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
                    }
                    else
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

                int staminaCost = 10;

                await HattingtonDbEditor.DeductFromCharacterStamina(discordUser, staminaCost, db);

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
                    // should now take into account if exp is high enough to get multiple level ups
                    while (newExperience >= expNeededForLevelUp)
                    {
                        characterLeveledUp = true;

                        // is replacement for below code
                        await HattingtonDbEditor.LevelUpCharacter(discordUser, db);

                        character.Experience = newExperience - expNeededForLevelUp;

                        newExperience = character.Experience;

                        // recalculate exp needed for levelup using new character level
                        expNeededForLevelUp = CalculateExpNeededForLevelUp(character.Level);
                    }

                    character.Experience = newExperience;

                }

                await db.SaveChangesAsync();

                return new FightLog
                {
                    IsValid = true,
                    Events = fightEvents,
                    PlayerEndHealth = playerCurrHealth,
                    PlayerFirst = playerInitiative,
                    PlayerWin = playerWin,
                    Character = character,
                    EnemyName = chosenEnemy.Name,
                    ExpGained = expGained,
                    DamageTakenByPlayer = damageTakenByPlayer,
                    DamageTakenByEnemy = damageTakenByEnemy,
                    CharacterLeveledUp = characterLeveledUp,
                };
            }
        }

        public static int CalculateExpNeededForLevelUp(int level)
        {
            return 50 + 50 * (level - 1);
        }

        static Enemy GetChosenEnemy(HatCharacter character)
        {
            using (var db = new Hattington())
            {
                // choose random enemy of appropriate rank
                var rank = db.EnemyRanks.Where(r => (character.Level >= r.MinLevel && character.Level <= r.MaxLevel)).FirstOrDefault();

                List<Enemy> enemies = db.Enemies.Where(e => e.EnemyRankID == rank.EnemyRankID).ToList();

                var rand = new Random();

                var randIndex = rand.Next(enemies.Count);

                return enemies[randIndex];
            }
        }

        static int CalculateDamageVariance(int attackingLevel, int defendingLevel)
        {
            var rand = new Random();

            int levelDifference = attackingLevel - defendingLevel;

            bool diffIsNegative = levelDifference < 0;

            if (diffIsNegative)
            {
                levelDifference = Math.Abs(levelDifference);
            }

            if (levelDifference == 0)
            {
                return 0;
            }

            int signMultiplier = diffIsNegative ? -1 : 1;

            int damageVariance = (int)Math.Floor(0.5 * rand.Next(levelDifference)) * signMultiplier;

            return damageVariance;
        }

    }
}

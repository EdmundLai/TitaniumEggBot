using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HattingtonGame.DbTypes;

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

        // Need to add field in database corresponding to actual user, so that
        // user can only have one character in the system
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

        // called by !hatfight being entered in the discord chat
        // TODO: add experience mechanic if player won battle
        // TODO: add rest call to allow player to regain hp
        // TODO: use stamina somehow
        public static async Task<FightLog> FightEnemy(string discordUser)
        {
            var character = HattingtonDbEditor.GetUserCharacter(discordUser);

            if (character == null)
            {
                return new FightLog
                {
                    IsValid = false,
                    Error = "Character does not exist! Create a character using !addhatchar characterName"
                };
            }

            using (var db = new Hattington())
            {
                if (character.Health == 0)
                {
                    // need to implement !rest
                    return new FightLog
                    {
                        IsValid = false,
                        Error = "Character cannot fight at 0 hp! Use !rest to regain hp!"
                    };
                }

                // Step 1: Get enemy from the corresponding pool
                Enemy chosenEnemy = GetChosenEnemy(character);

                // Step 2: initiate fight sequence

                // engine will set hp of player to new value if needed

                // don't need to set hp of enemy because new enemy will be fought every single time

                int playerCurrHealth = character.Health;

                int enemyCurrHealth = chosenEnemy.MaxHealth;

                var rand = new Random();

                bool fightNotOver = playerCurrHealth > 0 || enemyCurrHealth > 0;

                bool playerFirst = character.Level >= chosenEnemy.Level;

                bool playerWin = false;

                List<FightEvent> fightEvents = new List<FightEvent>();

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
                        int damage = Math.Max(isPhysAttack ? character.Attack + damageVariance - chosenEnemy.Defense : character.Magic + damageVariance - chosenEnemy.MagicDefense, 0);

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
                        int damage = Math.Max(isPhysAttack ? chosenEnemy.Attack + damageVariance - character.Defense : chosenEnemy.Magic + damageVariance - character.MagicDefense, 0);

                        playerCurrHealth -= damage;

                        // curr health cannot be negative
                        playerCurrHealth = Math.Max(playerCurrHealth, 0);

                        string physOrMagic = isPhysAttack ? "Physical" : "Magic";

                        return $"{chosenEnemy.Name} used a {physOrMagic} attack on the player for {damage} damage!";
                    }

                    //higher level opponent attacks first, equal level player and enemy gives priority to player
                    if (playerFirst)
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

                // save player health to db
                character.Health = playerCurrHealth;
                await db.SaveChangesAsync();

                return new FightLog
                {
                    IsValid = true,
                    Events = fightEvents,
                    PlayerEndHealth = playerCurrHealth,
                    PlayerFirst = playerFirst,
                    PlayerWin = playerWin,
                    CharacterName = character.CharacterName,
                    EnemyName = chosenEnemy.Name,
                };
            }
        }


        

        public static Enemy GetChosenEnemy(HatCharacter character)
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

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using TitaniumEggBot.Rotmg;

namespace TitaniumEggBot
{
    public class RotmgApiHelper
    {

        public async static Task<IDocument> GetDocumentAsync(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);

            return document;
        }

        public static string GetStarRanking(int numStars)
        {
            if(numStars <= 15 && numStars >= 1)
            {
                return "Cyan";
            } else if(numStars <= 31)
            {
                return "Blue";
            } else if(numStars <= 47)
            {
                return "Red";
            } else if(numStars <= 63)
            {
                return "Orange";
            } else if(numStars <= 79)
            {
                return "Yellow";
            } else if(numStars == 80)
            {
                return "White";
            } else
            {
                return "Invalid";
            }
        }

        public async static Task<RotmgPlayer> GetCharacterInfoInListAsync(string player)
        {
            var address = $"https://www.realmeye.com/player/{player}";
            var document = await GetDocumentAsync(address);
            string playerName = document.QuerySelector(".entity-name").TextContent;
            string rank = document.QuerySelector(".star-container").TextContent;

            var tableBody = document.QuerySelector("#e tbody");

            // If TextContent doesn't end with a /8 it isn't valid
            if (!tableBody.TextContent.EndsWith("/8"))
            {
                return null;
            }
            else
            {
                List<RotmgCharacter> characters = new List<RotmgCharacter>();

                var playerCharacterNodes = tableBody.ChildNodes;
                foreach (var charNode in playerCharacterNodes)
                {
                    var charInfoRows = charNode.ChildNodes;
                    //sb.Append($"{charNode.TextContent}\n");
                    string rotmgClassName = charInfoRows[2].TextContent;
                    string classLevel = charInfoRows[3].TextContent;
                    string fame = charInfoRows[5].TextContent;
                    string rankedPlace = charInfoRows[7].TextContent;
                    
                    var charItems = charInfoRows[8].ChildNodes;
                    string stats = charInfoRows[9].TextContent;

                    RotmgEquipment equipment = new RotmgEquipment();

                    for(int i = 0; i < charItems.Length; i++)
                    {
                        var itemNode = charItems[i];
                        var itemInfo = (IElement)itemNode.FirstChild.FirstChild;

                        string itemName = itemInfo.GetAttribute("title");

                        switch(i)
                        {
                            case 0:
                                equipment.Weapon = itemName;
                                break;
                            case 1:
                                equipment.Ability = itemName;
                                break;
                            case 2:
                                equipment.Armor = itemName;
                                break;
                            case 3:
                                equipment.Ring = itemName;
                                break;
                            default:
                                equipment.Backpack = itemName;
                                break;
                        }
                    }

                    int numLevel;
                    int numRankedPlace;
                    int numFame;

                    try
                    {
                        numLevel = Int32.Parse(classLevel);
                        numRankedPlace = Int32.Parse(rankedPlace);
                        numFame = Int32.Parse(fame);
                    }
                    catch
                    {
                        Console.WriteLine("either classLevel or ranked place (or both) was/were not an integer");
                        numLevel = -1;
                        numRankedPlace = -1;
                        numFame = -1;
                    }


                    RotmgCharacter character = new RotmgCharacter
                    {
                        Class = rotmgClassName,
                        Level = numLevel,
                        RankedPlace = numRankedPlace,
                        Fame = numFame,
                        Stats = stats,
                        Equipment = equipment
                    };

                    characters.Add(character);
                }

                int numRank;

                try
                {
                    numRank = Int32.Parse(rank);
                }
                catch
                {
                    Console.WriteLine("rank was not an integer");
                    numRank = -1;
                }


                RotmgPlayer playerInfo = new RotmgPlayer
                {
                    PlayerName = playerName,
                    RealmeyeURL = address,
                    Rank = numRank,
                    Characters = characters
                };

                return playerInfo;
            }
        }
    }
}

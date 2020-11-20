using Discord;
using Discord.Commands;
using HattingtonGame;
using HattingtonGame.DbTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TitaniumEggBot.Modules
{
    class HattingtonUtilities
    {
        // Small helper method to reduce redundant code
        public static Embed CreateEmbed(string title, List<EmbedFieldBuilder> embedFields, SocketCommandContext context)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = title,
                Fields = embedFields
            };

            Embed embed = embedBuilder
                .WithAuthor(context.Client.CurrentUser)
                .WithCurrentTimestamp()
                .Build();

            return embed;
        }

        public static string GenerateCharacterDescription(HatCharacter character)
        {
            return $"Level: {character.Level}\n" +
                        $"Experience: {character.Experience}\n" +
                        $"Hat: {HattingtonGameEngine.GetHatName(character.HatID)}\n" +
                        $"Health: {character.Health}/{character.MaxHealth}\n" +
                        $"Attack: {character.Attack}\n" +
                        $"Defense: {character.Defense}\n" +
                        $"Magic: {character.Magic}\n" +
                        $"Magic Defense: {character.MagicDefense}\n" +
                        $"Stamina: {character.Stamina}/{character.MaxStamina}\n" +
                        $"Fullness: {character.Fullness}/{character.MaxFullness}";
        }
    }
}

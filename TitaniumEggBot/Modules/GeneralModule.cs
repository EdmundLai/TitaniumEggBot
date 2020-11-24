using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumEggBot.Modules
{
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public GeneralModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Summary("Gets all commands available for use")]
        public async Task HelpAsync()
        {
            List<CommandInfo> commands = _commandService.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }

    }
}

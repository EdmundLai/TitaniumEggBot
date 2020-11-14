using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TitaniumEggBot.Modules
{
    // Create a module with no prefix
    public class DiscordApiTestModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("testiuser")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            return ReplyAsync(echo);
        }
        // ReplyAsync is a method on ModuleBase 
    }
}

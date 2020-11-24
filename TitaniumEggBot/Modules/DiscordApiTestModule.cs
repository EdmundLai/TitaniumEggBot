using Discord;
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
        // ReplyAsync is a method on ModuleBase 

        [Command("userinfo")]
        [Summary("Returns the user")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }
    }
}

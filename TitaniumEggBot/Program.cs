using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TitaniumEggBot
{
    class Program
    {
        //private readonly DiscordSocketClient _client;

        //private readonly CommandService _commands;

        //private readonly IServiceProvider _services;

        public static IConfigurationRoot _configuration { get; set; }

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        //private Program()
        //{
            //_client = new DiscordSocketClient(new DiscordSocketConfig
            //{
            //    LogLevel = LogSeverity.Info,
            //});

            //_commands = new CommandService(new CommandServiceConfig
            //{
            //    LogLevel = LogSeverity.Info,
            //    CaseSensitiveCommands = false,
            //});

            //_client.Log += Log;

            //_commands.Log += Log;

            // Setup your DI container
            //_services = this.ConfigureServices();
        //}

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var appSecrets = services.GetRequiredService<AppSecrets>();

                var handler = services.GetRequiredService<CommandHandler>();
                await handler.InstallCommandsAsync();

                var client = services.GetRequiredService<DiscordSocketClient>();
                var commandService = services.GetRequiredService<CommandService>();

                client.Log += Log;
                commandService.Log += Log;

                //  You can assign your bot token to a string, and pass that in to connect.
                //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.

                // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
                await client.LoginAsync(TokenType.Bot, appSecrets.DiscordToken);
                await client.StartAsync();

                await Task.Delay(-1);
            }
            
        }

        // If any services require the client, or the CommandService, or something else you keep on hand,
        // pass them as parameters into this method as needed.
        // If this method is getting pretty long, you can seperate it out into another file using partials.
        private ServiceProvider ConfigureServices()
        {
            _configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

            //string token = File.ReadAllText("lolApiToken.txt");
            Console.WriteLine(Directory.GetCurrentDirectory());
            var map = new ServiceCollection()
                .AddSingleton(new AppSecrets
                {
                    DiscordToken = _configuration.GetSection("discordToken").Value,
                    LolApiToken = _configuration.GetSection("lolApiToken").Value
                })
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LolApiHelper>();
            //.AddSingleton(new SomeServiceClass());
            // Repeat this for all the service classes
            // and other dependencies that your commands might need.


            // When all your required services are in the collection, build the container.
            // Tip: There's an overload taking in a 'validateScopes' bool to make sure
            // you haven't made any mistakes in your dependency graph.
            return map.BuildServiceProvider();
        }


        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }



    }
}

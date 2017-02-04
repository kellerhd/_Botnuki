using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using _Botnuki.Modules.Admin;
using Discord;

namespace _Botnuki
{
    public class CommandHandler
    {
        static private CommandService commands;
        static private DiscordSocketClient client;
        static private IDependencyMap map;

        public async Task Install(IDependencyMap _map)
        {
            try
            {
                // Create Command Service, inject it into Dependency Map
                client = _map.Get<DiscordSocketClient>();
                commands = new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false
                });
                _map.Add(commands);
                map = _map;

                await commands.AddModuleAsync<RolesModule>().ConfigureAwait(false);
                Console.WriteLine("Roles Module Installed!");
                //await commands.AddModuleAsync<BanModule>().ConfigureAwait(false);
                client.MessageReceived += HandleCommand;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ErrorHandling.ThrowGenException("CommandHandler.cs", "Install(IDependencyMap)", ex.Message));
                return; 
            }
        }

        public static async Task HandleCommand(SocketMessage parameterMessage)
        {
            try
            {
                // Don't handle the command if it is a system message
                var message = parameterMessage as SocketUserMessage;
                if (message == null) return;

                // Mark where the prefix ends and the command begins
                int argPos = 0;
                // Determine if the message has a valid prefix, adjust argPos 
                if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasCharPrefix('/', ref argPos))) return;

                // Create a Command Context
                var context = new CommandContext(client, message);
                // Execute the Command, store the result
                var result = await commands.ExecuteAsync(context, argPos, map);

                // If the command failed, notify the user
                if (!result.IsSuccess)
                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ErrorHandling.ThrowGenException("CommandHandler.cs", "HandleCommand(SocketMessage)", ex.Message));
                return;
            }
        }
    }
}

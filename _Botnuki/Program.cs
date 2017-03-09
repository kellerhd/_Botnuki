using System;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;

namespace _Botnuki
{
    public class Program
    {
        static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler comms;

        public async Task Start()
        {
            try
            {
                Console.WriteLine("Welcome to Botnuki v2.0! Initiating. . .");
                // instantiate the client
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    WebSocketProvider = WS4NetProvider.Instance
                });

                // get the server token
                Console.WriteLine("Finding server path. . .");
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.Length - 10);
                var token = File.ReadAllText($@"{path}inukitvsrvid.txt");

                // log in and connect
                Console.WriteLine("Connecting to server. . .");
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                // dependency map
                var map = new DependencyMap();
                map.Add(_client);

                // install commands using CommandHandler
                Console.WriteLine("Installing commands. . .");
                comms = new CommandHandler();
                await comms.Install(map);

                // block until program is closed
                Console.WriteLine("Connected!");
                await Task.Delay(-1);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ErrorHandling.ThrowGenException("Program.cs", "Start()", ex.Message));
                Console.ReadKey();
            }
        }

        private Task Log(LogMessage msg)
        {
            try
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ErrorHandling.ThrowGenException("Program.cs", "Log(LogMessage)", ex.Message));
                return null;
            }
        }

    }
}

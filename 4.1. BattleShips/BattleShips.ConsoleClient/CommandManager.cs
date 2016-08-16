using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.ConsoleClient
{
    using System.Net.Http;

    using Battleships.Data;
    using Battleships.WebServices.Models;

    using BattleShips.ConsoleClient.Models;

    public class CommandManager
    {
        private const string RegisterEndPoint = "http://localhost:62859/api/account/register";

        private const string LoginEndPoint = "http://localhost:62859/token";

        private const string GreateGameEndPoint = "http://localhost:62859/api/games/create";

        private const string JoinGameEndPoint = "http://localhost:62859/api/games/join";

        private const string PlayGameEndPoint = "http://localhost:62859/api/games/play";


        private IBattleshipsData database;

        private string token;

        public CommandManager(IBattleshipsData database)
        {
            this.database = database;
        }

        public void ExecuteCommand(string inputLine)
        {
            var commandParams = inputLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var command = commandParams[1];

            switch (command.ToLower())
            {
                case "register":
                    this.RegisterAsync(commandParams);
                    break;
                case "login":
                    this.LoginAsync(commandParams);
                    break;
                case "create-game":
                    this.CreateGameAsync();
                    break;
                case "join-game":
                    this.JoinGameAsync(commandParams);
                    break;
                case "play":
                    this.PlayGameAsync(commandParams);
                    break;
                default:
                    throw new InvalidOperationException("Invalid command");
                
            }

        }


        private async Task PlayGameAsync(string[] commandParams)
        {
            var gameId = commandParams[2];
            var x = commandParams[3];
            var y = commandParams[4];

            var client = new HttpClient();
            client.DefaultRequestHeaders
               .Add("Authorization", "Bearer " + this.token);

            var content = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("gameId", gameId),
                new KeyValuePair<string, string>("positionX", x),
                new KeyValuePair<string, string>("positionY", y),   
            });

            var response = await client.PostAsync(PlayGameEndPoint, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        private async Task JoinGameAsync(string[] commandParams)
        {


            var gameId = commandParams[2];

            var client = new HttpClient();
            client.DefaultRequestHeaders
               .Add("Authorization", "Bearer " + this.token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("gameId", gameId), 
            });

            var response = await client.PostAsync(JoinGameEndPoint, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
                Console.WriteLine("Logged successfully in game with id {0}", gameId);
            }

        }

        private async Task CreateGameAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders
                .Add("Authorization", "Bearer " + this.token);

            var response = await client.PostAsync(GreateGameEndPoint, null);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
            }

        }

        private async Task LoginAsync(string[] commandParams)
        {
            if (this.token != null)
            {
                Console.WriteLine("Already logged in");
            }

            var username = commandParams[2];
            var password = commandParams[3];

            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password"),                                           
            });

            var response = await client.PostAsync(LoginEndPoint, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                var accToken = await response.Content.ReadAsAsync<LoginDTO>();
                this.token = accToken.Access_Token;
                Console.WriteLine("User {0} successfully logged in.", username);
            }


        }

        private async Task RegisterAsync(string[] commandParams)
        {
            string result;
            var email = commandParams[2];
            var password = commandParams[3];
            var confirmPassword = commandParams[4];

            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email), 
                new KeyValuePair<string, string>("password", password), 
                new KeyValuePair<string, string>("confirmPassword", confirmPassword),   
            });

            var response = await client.PostAsync(RegisterEndPoint, content);

            if (!response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                result = response.StatusCode.ToString();
            }

            Console.WriteLine(result);
        }
    }
}

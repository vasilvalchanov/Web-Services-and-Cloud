using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.ConsoleClient
{
    public class Engine
    {
        private CommandManager commandManager;

        public Engine(CommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        public void Run()
        {
            var isRunning = true;

            while (isRunning)
            {
                var inputLine = Console.ReadLine();

                if (inputLine == "end")
                {
                    isRunning = false;
                }
                else
                {
                    this.commandManager.ExecuteCommand(inputLine);
                    
                }

            }
        }
    }
}

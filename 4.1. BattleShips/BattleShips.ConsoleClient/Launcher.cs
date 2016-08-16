using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShips.ConsoleClient
{
    using Battleships.Data;

    class Launcher
    {
        static void Main(string[] args)
        {
            var database = new BattleshipsData(new ApplicationDbContext());
            var commandManager = new CommandManager(database);
            var engine = new Engine(commandManager);
            engine.Run();

        }
    }
}

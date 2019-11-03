using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            while (game.isRunning)
            {
                // Do nothing (handled all within game)
            }
            // Game shutdown.  Thanks for playing!
        }
    }
}

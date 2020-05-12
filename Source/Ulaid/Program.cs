#if !DEBUG
using System;
#endif

namespace Ulaid
{
    class Program
    {
        public static UlaidGame GameInstance { get; set; }

        static void Main(string[] args)
        {
            GameInstance = new UlaidGame("GameProperties.json");
            GameInstance.Run();
            while (GameInstance.isRunning) ;
            // Game has shutdown at this point
        }
    }
}

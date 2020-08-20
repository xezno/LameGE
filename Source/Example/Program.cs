#if !DEBUG
using System;
#endif

namespace Example
{
    class Program
    {
        public static ExampleGame GameInstance { get; set; }

        static void Main(string[] args)
        {
            GameInstance = new ExampleGame("GameProperties.json");
            GameInstance.Run();
            while (GameInstance.isRunning) { }
            // Game has shutdown at this point
        }
    }
}

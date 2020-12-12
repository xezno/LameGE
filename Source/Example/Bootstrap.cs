#if !DEBUG
using System;
#endif

namespace Example
{
    class Bootstrap
    {
        public static ExampleGame GameInstance { get; set; }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            GameInstance = new ExampleGame("GameProperties.json");
            GameInstance.Run();
            while (GameInstance.isRunning) ;
        }
    }
}

namespace ECSEngine
{
    internal class Program
    {
        // TODO: think of a better way to access game like this
        public static Game game;
        static void Main(string[] args)
        {
            game = new Game();
            game.Run();
            while (game.isRunning)
            {
                // Do nothing (handled all within Game)
            }
            // Game shutdown.  Thanks for playing!
        }
    }
}

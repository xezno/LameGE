namespace SpaceGame
{
    class Program
    {
        // TODO: think of a better way to access game like this
        public static SpaceGame game;
        static void Main(string[] args)
        {
            game = new SpaceGame();
            game.Run();
            while (game.isRunning)
            {
                // Do nothing (handled all within SpaceGame)
            }
            // Game shutdown.  Thanks for playing!
        }
    }
}

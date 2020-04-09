namespace UlaidGame
{
    class Program
    {
        public static UlaidGame GameInstance { get; set; }

        static void Main(string[] args)
        {
            GameInstance = new UlaidGame("GameProperties.json");

#if DEBUG
            GameInstance.Run();
#else
            try
            {
                GameInstance.Run();
            }
            catch (Exception e)
            {
                Debug.Log($"FATAL ERROR: {e.ToString()}", DebugSeverity.Fatal);
            }
#endif
            while (GameInstance.isRunning) ;
            // Game has shutdown at this point
        }
    }
}

using Engine.Utils.DebugUtils;
using Newtonsoft.Json;

namespace Engine.Utils
{
    struct GameSettingsValues
    {
        public int gameResolutionX;
        public int gameResolutionY;
        public bool vsyncEnabled;
        public int framerateLimit;
        public int gamePosX;
        public int gamePosY;
        public bool fullscreen;
        public string rconPassword;
        public bool rconEnabled;
        public int rconPort;
        public int webPort;
        public float physTimeStep;
        public float updateTimeStep;

        // TODO: cascades
        public int shadowMapX;
        public int shadowMapY;
    }

    public sealed class GameSettings
    {
        private static GameSettingsValues values;

        public static int GameResolutionX { get => values.gameResolutionX; }
        public static int GameResolutionY { get => values.gameResolutionY; }
        public static bool VsyncEnabled { get => values.vsyncEnabled; }
        public static int FramerateLimit { get => values.framerateLimit; }
        public static int GamePosX { get => values.gamePosX; }
        public static int GamePosY { get => values.gamePosY; }
        public static bool Fullscreen { get => values.fullscreen; }
        public static string RconPassword { get => values.rconPassword; }
        public static bool RconEnabled { get => values.rconEnabled; }
        public static int RconPort { get => values.rconPort; }
        public static int WebPort { get => values.webPort; }
        public static float PhysTimeStep { get => values.physTimeStep; }
        public static float UpdateTimeStep { get => values.updateTimeStep; }
        public static int ShadowMapX { get => values.shadowMapX; }
        public static int ShadowMapY { get => values.shadowMapY; }

        public static void LoadValues()
        {
            Logging.Log("Loading game settings");
            var fileContents = System.IO.File.ReadAllText("GameSettings.json");
            Logging.Log(fileContents);
            values = JsonConvert.DeserializeObject<GameSettingsValues>(fileContents);
        }
    }
}

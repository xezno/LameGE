using ECSEngine;

namespace SpaceGame
{
    internal sealed class SpaceGame : Game
    {
        // TODO: Move this to a completely data-based solution where the game's name, window title, etc. are all just loaded from a JSON file.
        public new void Run()
        {
            base.Run();

            nativeWindow.Caption = "SpaceGame";
        }
    }
}

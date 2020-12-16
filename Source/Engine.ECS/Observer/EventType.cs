namespace Engine.ECS.Observer
{
    /// <summary>
    /// An enumeration type containing all possible events that may occur within a game instance.
    /// </summary>
    public enum NotifyType
    {
        //--------------------------------
        /* GL context */
        ContextReady,

        //--------------------------------
        /* Game events */
        GameEnd,

        //--------------------------------
        /* Game Input */
        // Keyboard
        KeyUp,
        KeyDown,

        // Mouse
        MouseMove,
        MouseButtonUp,
        MouseButtonDown,
        MouseScroll,

        //--------------------------------
        /* Editor Input */
        // Keyboard
        EditorKeyUp,
        EditorKeyDown,

        // Mouse
        EditorMouseMove,
        EditorMouseButtonUp,
        EditorMouseButtonDown,
        EditorMouseScroll,

        //--------------------------------
        /* Native window */
        WindowResized,
        CloseGame
    }
}

namespace Engine.Events
{
    /// <summary>
    /// An enumerator containing all possible events that may occur within a game instance.
    /// </summary>
    public enum Event
    {
        GameStart,
        GameEnd,

        KeyUp,
        KeyDown,

        MouseMove,
        MouseButtonUp,
        MouseButtonDown,
        MouseScroll,

        WindowResized
    }
}

namespace ECSEngine.DebugUtils
{
    enum RconPacketType
    {
        Handshake,
        Input,
        Response,
        InputInProgress,
        Suggestions,
        LogHistory,
        Error = 0xFF
    }
}

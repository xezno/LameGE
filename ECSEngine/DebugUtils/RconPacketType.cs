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
        RequestAuth,
        Authenticate,
        Error = 0xFF
    }
}

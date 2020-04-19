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
        RequestLogHistory,
        Error = 0xFF
    }
}

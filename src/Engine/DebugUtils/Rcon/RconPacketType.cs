namespace Engine.DebugUtils.Rcon
{
    enum RconPacketType
    {
        Handshake,
        Input,
        Response,
        InputInProgress,
        Suggestions,
        RequestAuth,
        Authenticate,
        RequestLogHistory,
        LogHistory,
        Error = 255
    }
}

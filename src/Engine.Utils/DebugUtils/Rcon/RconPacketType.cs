namespace Engine.Utils.DebugUtils.Rcon
{
    public enum RconPacketType
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

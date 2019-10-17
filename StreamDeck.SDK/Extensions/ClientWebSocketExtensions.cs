namespace System.Net.WebSockets
{
    public static class ClientWebSocketExtensions
    {
        public static bool IsAvailable(this ClientWebSocket socket)
        {
            switch (socket.State)
            {
                case WebSocketState.Open: return true;
                default: return false;
            }
        }
    }
}

namespace ChatApp.Common
{
    public static class ConnectionIdHelper
    {
        private static readonly Dictionary<int, string> _connections = new();

        public static void AddConnection(int userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public static string? GetConnectionId(int userId)
        {
            return _connections.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }

        public static void RemoveConnection(int userId)
        {
            if (_connections.ContainsKey(userId))
                _connections.Remove(userId);
        }
    }
}

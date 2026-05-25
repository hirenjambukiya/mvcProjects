namespace chatApp.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public bool IsConnected { get; set; }

        public DateTime ConnectedAt { get; set; } = DateTime.Now;
    }
}

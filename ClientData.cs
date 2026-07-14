namespace HWExtra_Server
{
    public class ClientData
    {
        public string Name { get; set; } = "";
        public string Secret { get; set; } = "";
        public Dictionary<string, string> Friends { get; set; } = new();
        public string? Server { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsOnline() => (DateTime.UtcNow - LastSeen).TotalSeconds < 60;
    }
}

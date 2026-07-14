using System;
using System.Collections.Generic;
using System.Text;

namespace HWExtra_Server.Models
{
    public class ClientModel
    {
        public string Username { get; set; } = "";
        public List<string> FriendNames { get; set; } = new();
        public string? Server { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsOnline() => (DateTime.UtcNow - LastSeen).TotalSeconds < 60;
    }
}

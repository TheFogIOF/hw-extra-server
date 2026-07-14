using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HWExtra_Server.Requests
{
    public class GetRequest
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; } = null;
        [JsonPropertyName("server")]
        public string Server { get; set; } = "";
        [JsonPropertyName("friend_names")]
        public List<string> FriendNames { get; set; } = new();
    }
}

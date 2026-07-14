using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HWExtra_Server.Requests
{
    public class LeaveRequest
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; } = null;
    }
}

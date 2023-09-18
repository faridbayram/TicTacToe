using System;

namespace wsServer.Models
{
    public class LostOnTimeRequest
    {
        public string UserId { get; set; }
        public Guid GameId { get; set; }
    }
}
using System;

namespace wsServer.Models
{
    public class StartGameResponse
    {
        public Guid GameId { get; set; }
        public string YourMove { get; set; }
    }
}
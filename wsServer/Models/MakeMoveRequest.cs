using System;

namespace wsServer.Models
{
    public class MakeMoveRequest
    {
        public Guid GameId { get; set; }
        public int CellIndex { get; set; }
        public string Move { get; set; }
    }
}
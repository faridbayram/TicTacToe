namespace wsServer.Models
{
    public class MakeMoveResponse
    {
        public string CurrentMove { get; set; }
        public int PreviouslyFilledCellIndex { get; set; }
        public string PreviouslyPlayedMove { get; set; }
    }
}
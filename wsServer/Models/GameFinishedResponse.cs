namespace wsServer.Models
{
    public class GameFinishedResponse
    {
        public string Message { get; set; }
        public int PreviouslyFilledCellIndex { get; set; }
        public string PreviouslyPlayedMove { get; set; }
        public int[] WinPattern { get; set; }
        public string WinnerId { get; set; }
        public string Reason { get; set; }
    }
}
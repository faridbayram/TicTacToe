namespace wsServer.Models
{
    public class ClientResponse
    {
        public string ResponseType { get; set; }
        public StartGameResponse StartGameResponse { get; set; }
        public MakeMoveResponse MakeMoveResponse { get; set; }
        public GameFinishedResponse GameFinishedResponse { get; set; }
    }
}
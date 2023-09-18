namespace wsServer.Models
{
    public class ClientRequest
    {
        public string RequestType { get; set; }
        public SetMyIdRequest SetMyIdRequest { get; set; }
        public MakeMoveRequest MakeMoveRequest { get; set; }
        public LostOnTimeRequest LostOnTimeRequest { get; set; }
    }
}
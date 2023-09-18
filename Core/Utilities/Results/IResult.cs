namespace Core.Utilities.Results
{
    public interface IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
    }
}
namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }

        public Result(bool isSucceeded, string message) : this(isSucceeded)
        {
            Message = message;
        }

        public Result(bool isSucceeded)
        {
            IsSucceeded = isSucceeded;
        }
    }
}
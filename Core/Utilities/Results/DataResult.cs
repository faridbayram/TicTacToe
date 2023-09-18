namespace Core.Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T Data { get; set; }

        protected DataResult(T data, bool isSucceeded) : base(isSucceeded)
        {
            Data = data;
        }

        protected DataResult(T data, bool isSucceeded, string message) : base(isSucceeded, message)
        {
            Data = data;
        }
    }
}
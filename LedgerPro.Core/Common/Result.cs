namespace LedgerPro.Core.Common
{
    /// <summary>
    /// A generic Result class to represent the outcome of an operation, 
    /// encapsulating success/failure status, a value (if successful), and an error message (if failed).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public string Error { get; }

        /// <summary>
        /// Private constructor to enforce the use of factory methods for creating instances of Result.
        /// </summary>
        /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="value">The value associated with the result (if successful).</param>
        /// <param name="error">The error message (if failed).</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected Result(bool success, T? value, string error)
        {
            if (success && error != string.Empty)
                throw new InvalidOperationException("Successful result cannot have an error message.");

            if (!success && value != null)
                throw new InvalidOperationException("Failed result cannot have a value.");

            IsSuccess = success;
            Value = value;
            Error = error;
        }

        // Factory methods for creating success and failure results
        public static Result<T> Success(T value) => new Result<T>(true, value, string.Empty);
        public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    }
}
using System.Net;

namespace ContactManagement.Shared.Common
{
    public class Result<T>
    {
        protected Result(T? value, Error? error, HttpStatusCode statusCode)
        {
            Value = value;
            Error = error;
            StatusCode = statusCode;
            IsSuccess = error == null;
        }

        public T? Value { get; }
        public Error? Error { get; }
        public HttpStatusCode StatusCode { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public static Result<T> FromResult<TSource>(Result<TSource> result) => new(default, result.Error, result.StatusCode);

        public static Result<T> Success(T value) => new(value, null, HttpStatusCode.OK);

        public static Result<T> BadRequest(Error error) => new(default, error, HttpStatusCode.BadRequest);

        public static Result<T> NotFound(Error error) => new(default, error, HttpStatusCode.NotFound);

        public static Result<T> Conflict(Error error) => new(default, error, HttpStatusCode.Conflict);

        public static Result<T> Unauthorized(Error error) => new(default, error, HttpStatusCode.Unauthorized);

        public static Result<T> Forbidden(Error error) => new(default, error, HttpStatusCode.Forbidden);

        public static Result<T> ValidationError(Error error) => new(default, error, HttpStatusCode.UnprocessableEntity);

        public static Result<T> InternalServerError(Error error) => new(default, error, HttpStatusCode.InternalServerError);
    }
}
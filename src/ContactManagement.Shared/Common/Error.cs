namespace ContactManagement.Shared.Common
{
    public sealed class Error(string code, string message)
    {
        public string Code { get; } = code;
        public string Message { get; } = message;

        public static implicit operator string(Error error) => error.Code;
    }
}


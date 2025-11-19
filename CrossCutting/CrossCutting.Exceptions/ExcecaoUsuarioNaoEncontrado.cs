namespace CrossCutting.Exceptions
{
    public class ExcecaoUsuarioNaoEncontrado : Exception
    {
        public int StatusCode { get; } = 400;

        public ExcecaoUsuarioNaoEncontrado(string message)
            : base(message) { }


        public ExcecaoUsuarioNaoEncontrado(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

    }
}
